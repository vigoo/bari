using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Bari.Core.Exceptions;
using Bari.Core.Generic;
using Bari.Core.Model.Parameters;
using Bari.Core.Model.Validator;
using Bari.Core.UI;
using YamlDotNet.RepresentationModel;

namespace Bari.Core.Model.Loader
{
    /// <summary>
    /// Base class for YAML based loaders 
    /// </summary>
    public abstract class YamlModelLoaderBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(YamlModelLoaderBase));

        private readonly ISuiteFactory suiteFactory;
        private readonly IEnumerable<IYamlProjectParametersLoader> parametersLoaders;
        private readonly IUserOutput output;
        private readonly YamlParser parser;
        private readonly ReferenceLoader referenceLoader = new ReferenceLoader();
        private readonly IEnvironmentVariableContext versioningEnvironmentVariableContext;
        private readonly IPluginLoader pluginLoader;
        private readonly ISuiteValidator validator;

        /// <summary>
        /// Initializes the yaml loader
        /// </summary>
        /// <param name="suiteFactory">Factory interface to create new suite instances</param>
        /// <param name="parametersLoaders">Parameter loader implementations</param>
        /// <param name="output">Output interface to issue warnings</param>
        /// <param name="pluginLoader">Plugin loader interface</param>
        /// <param name="environmentVariableContext">Environment variable context</param>
        /// <param name="validator">Suite validator interface</param>
        protected YamlModelLoaderBase(ISuiteFactory suiteFactory, IEnumerable<IYamlProjectParametersLoader> parametersLoaders, IUserOutput output, IPluginLoader pluginLoader, IEnvironmentVariableContext environmentVariableContext, ISuiteValidator validator)
        {
            Contract.Requires(suiteFactory != null);
            Contract.Requires(output != null);
            Contract.Requires(pluginLoader != null);
            Contract.Requires(environmentVariableContext != null);
            Contract.Requires(validator != null);
            Contract.Ensures(this.suiteFactory == suiteFactory);
            Contract.Ensures(this.parametersLoaders == parametersLoaders);

            this.suiteFactory = suiteFactory;
            this.parametersLoaders = parametersLoaders;
            this.output = output;
            this.pluginLoader = pluginLoader;
            this.validator = validator;

            versioningEnvironmentVariableContext = new VersioningEnvironmentVariableContext(environmentVariableContext);
            parser = new YamlParser();
        }

        /// <summary>
        /// Process an already loaded <c>YamlDocument</c> and returns the loaded suite model.
        /// </summary>
        /// <param name="yaml">The yaml document to process.</param>
        /// <returns>Returns a loaded model if succeeds. On error it throws an exception, never
        /// returns <c>null</c>.</returns>
        protected Suite LoadYaml(YamlDocument yaml)
        {
            Contract.Requires(yaml != null);
            Contract.Requires(yaml.RootNode != null);            
            Contract.Ensures(Contract.Result<Suite>() != null);

            log.Debug("Processing YAML document...");

            foreach (var pluginUri in LoadPlugins(yaml.RootNode))
                pluginLoader.Load(pluginUri);

            var goals = LoadGoals(yaml.RootNode);
            var defaultGoal = LoadDefaultGoal(goals, yaml.RootNode) ?? Suite.DebugGoal;
            var suite = suiteFactory.CreateSuite(new HashSet<Goal>(goals.Values), defaultGoal);

            parser.SetActiveGoal(suite.ActiveGoal);

            suite.Name = parser.GetScalarValue(yaml.RootNode, "suite", "Error reading the name of the suite");
            suite.Version = ParseVersion(parser.GetOptionalScalarValue(yaml.RootNode, "version", null));
            suite.Copyright = parser.GetOptionalScalarValue(yaml.RootNode, "copyright", null);

            LoadParameters(suite, suite, yaml.RootNode);

            foreach (KeyValuePair<string, YamlNode> item in parser.EnumerateNamedNodesOf(yaml.RootNode, "modules"))
            {
                var module = suite.GetModule(item.Key);

                if (item.Value != null)
                    LoadModule(module, item.Value);
            }

            foreach (KeyValuePair<string, YamlNode> item in parser.EnumerateNamedNodesOf(yaml.RootNode, "products"))
            {
                var product = suite.GetProduct(item.Key);

                if (item.Value != null)
                    LoadProduct(suite, product, item.Value);
            }

            LoadSourceSetIgnoreLists(suite.SourceSetIgnoreLists, yaml.RootNode);

            validator.Validate(suite);

            log.Debug("Finished processing YAML document.");
            return suite;
        }

        private IEnumerable<Uri> LoadPlugins(YamlNode rootNode)
        {
            var result = new List<Uri>();

            var rootMapping = (YamlMappingNode) rootNode;
            YamlNode node;
            if (rootMapping.Children.TryGetValue(new YamlScalarNode("plugins"), out node))
            {
                var pluginsNode = node as YamlSequenceNode;
                if (pluginsNode != null)
                {
                    foreach (var item in parser.EnumerateNodesOf(pluginsNode))
                    {
                        var scalar = item as YamlScalarNode;
                        if (scalar != null)
                        {
                            try
                            {
                                result.Add(new Uri(scalar.Value));
                            }
                            catch (UriFormatException)
                            {
                                output.Warning(String.Format("Invalid plugin reference: {0}", scalar.Value));
                            }
                        }
                        else
                        {
                            output.Warning("Invalid child item ");
                        }
                    }
                }
            }

            return result;
        }

        private void LoadSourceSetIgnoreLists(SourceSetIgnoreLists ignoreLists, YamlNode rootNode)
        {
            foreach (var item in parser.EnumerateNamedNodesOf(rootNode, "ignore-lists"))
            {
                var ignoreList = ignoreLists.Get(item.Key);

                var mappedItem = (YamlMappingNode) item.Value;

                foreach (var expression in parser.EnumerateNodesOf((YamlSequenceNode)mappedItem.Children[new YamlScalarNode("ignore")]))
                {
                    ignoreList.Add(((YamlScalarNode)expression).Value);
                }
            }
        }

        private IDictionary<string, Goal> LoadGoals(YamlNode rootNode)
        {
            var result = new Dictionary<string, Goal>();
            foreach (KeyValuePair<string, YamlNode> item in parser.EnumerateNamedNodesOf(rootNode, "goals"))
            {
                var goal = LoadGoal(item.Key, item.Value, result);
                result.Add(goal.Name, goal);
            }

            return result;
        }

        private Goal LoadGoal(string name, YamlNode value, IDictionary<string, Goal> loadedGoals)
        {
            var mappingNode = value as YamlMappingNode;
            if (mappingNode != null)
            {
                var incorporates = new List<Goal>();
                foreach (var pair in parser.EnumerateNamedNodesOf(value, "incorporates"))
                {
                    string childName = pair.Key;

                    Goal g;
                    if (!loadedGoals.TryGetValue(childName, out g))
                    {
                        g = new Goal(childName);
                    }

                    incorporates.Add(g);
                }

                return new Goal(name, incorporates);
            }
            else
            {
                return new Goal(name);
            }
        }

        private Goal LoadDefaultGoal(IDictionary<string, Goal> goals, YamlNode rootNode)
        {
            var goalName = parser.GetOptionalScalarValue(rootNode, "default-goal", null);
            if (goalName != null)
            {
                if (goals.Count > 0)
                {
                    Goal goal;
                    if (goals.TryGetValue(goalName, out goal))
                        return goal;
                    else
                        throw new InvalidGoalException(goalName, new HashSet<Goal>(goals.Values));
                }
                else
                {
                    if (goalName == Suite.DebugGoal.Name)
                        return Suite.DebugGoal;
                    else if (goalName == Suite.ReleaseGoal.Name)
                        return Suite.ReleaseGoal;
                    else
                        throw new InvalidGoalException(goalName, new HashSet<Goal> { Suite.DebugGoal, Suite.ReleaseGoal });
                }
            }

            return null;
        }

        private void LoadParameters(Suite suite, IProjectParametersHolder target, YamlNode node)
        {
            var mapping = node as YamlMappingNode;
            if (mapping != null)
            {
                foreach (var pair in parser.EnumerateNodesOf(mapping))
                {
                    if ((new YamlScalarNode("postprocessors").Equals(pair.Key) ||
                         new YamlScalarNode("packager").Equals(pair.Key)) &&
                        pair.Value is YamlMappingNode)
                    {
                        // skipping
                    }
                    else
                    {
                        TryAddParameters(suite, target, pair.Key, pair.Value);
                    }
                }
            }
        }

        private void LoadPackager(Suite suite , Product product, YamlNode node)
        {
            var mapping = node as YamlMappingNode;
            if (mapping != null)
            {
                var packagerKey = new YamlScalarNode("packager");
                if (mapping.Children.ContainsKey(packagerKey))
                {
                    var packagerNode = mapping.Children[packagerKey] as YamlMappingNode;
                    if (packagerNode != null)
                    {
                        string type = parser.GetScalarValue(packagerNode, "type", "Packager type is not defined");

                        var loader = parametersLoaders.FirstOrDefault(l => l.Supports(type));
                        IPackagerParameters param = null;
                        if (loader != null)
                        {                            

                            YamlNode paramNode;
                            if (packagerNode.Children.TryGetValue(new YamlScalarNode("param"), out paramNode))
                            {
                                param = loader.Load(suite, type, paramNode, parser) as IPackagerParameters;
                            }
                            else
                            {
                                param = loader.Load(suite, type, new YamlMappingNode(), parser) as IPackagerParameters;
                            }
                        }

                        product.Packager = new PackagerDefinition(type, param);
                    }
                }
            }
        }

        private void LoadProduct(Suite suite, Product product, YamlNode productNode)
        {
            Contract.Requires(product != null);
            Contract.Requires(productNode != null);

            foreach (KeyValuePair<string, YamlNode> item in parser.EnumerateNamedNodesOf(productNode, "modules"))
            {
                if (suite.HasModule(item.Key))
                {
                    var module = suite.GetModule(item.Key);
                    product.AddModule(module);
                }
                else
                {
                    output.Warning(String.Format("The product {0} refers to a non-existing module {1}", product.Name, item.Key));
                }
            }

            SetProjectPostProcessors(suite, product, productNode);
            LoadParameters(suite, product, productNode);
            LoadPackager(suite, product, productNode);
        }

        private void LoadModule(Module module, YamlNode moduleNode)
        {
            Contract.Requires(module != null);
            Contract.Requires(moduleNode != null);

            LoadModuleVersion(module, moduleNode);
            LoadModuleCopyright(module, moduleNode);
            LoadParameters(module.Suite, module, moduleNode);
            LoadProjects(module, moduleNode);
            LoadTestProjects(module, moduleNode);
            SetProjectPostProcessors(module.Suite, module, moduleNode);
        }

        private void LoadModuleVersion(Module module, YamlNode moduleNode)
        {
            module.Version = ParseVersion(parser.GetOptionalScalarValue(moduleNode, "version", null));
        }

        private void LoadModuleCopyright(Module module, YamlNode moduleNode)
        {
            module.Copyright = parser.GetOptionalScalarValue(moduleNode, "copyright", null);
        }

        private void LoadTestProjects(Module module, YamlNode moduleNode)
        {
            foreach (KeyValuePair<string, YamlNode> item in parser.EnumerateNamedNodesOf(moduleNode, "tests"))
            {
                var testProject = module.GetTestProject(item.Key);

                if (item.Value != null)
                    LoadProject(module.Suite, testProject, item.Value);
            }
        }

        private void LoadProjects(Module module, YamlNode moduleNode)
        {
            foreach (KeyValuePair<string, YamlNode> item in parser.EnumerateNamedNodesOf(moduleNode, "projects"))
            {
                var project = module.GetProject(item.Key);

                if (item.Value != null)
                    LoadProject(module.Suite, project, item.Value);
            }
        }

        private void LoadProject(Suite suite, Project project, YamlNode projectNode)
        {
            Contract.Requires(suite != null);
            Contract.Requires(project != null);
            Contract.Requires(projectNode != null);

            var mapping = projectNode as YamlMappingNode;
            if (mapping != null)
            {
                foreach (var pair in parser.EnumerateNodesOf(mapping))
                {
                    if (new YamlScalarNode("type").Equals(pair.Key) &&
                        pair.Value is YamlScalarNode)
                    {
                        SetProjectType(project, ((YamlScalarNode)pair.Value).Value);
                    }
                    else if (new YamlScalarNode("version").Equals(pair.Key) &&
                    pair.Value is YamlScalarNode)
                    {
                        project.Version = ParseVersion(((YamlScalarNode)pair.Value).Value);
                    }
                    else if (new YamlScalarNode("copyright").Equals(pair.Key) && pair.Value is YamlScalarNode)
                    {
                        project.Copyright = ((YamlScalarNode)pair.Value).Value;
                    }
                    else if (new YamlScalarNode("references").Equals(pair.Key) &&
                        pair.Value is YamlSequenceNode)
                    {
                        SetProjectReferences(project, ((YamlSequenceNode)pair.Value).Children);
                    }
                    else if (new YamlScalarNode("postprocessors").Equals(pair.Key) &&
                             pair.Value is YamlMappingNode)
                    {
                        // skipping
                    }
                    else if (new YamlScalarNode("forced-order").Equals(pair.Key) && 
                             pair.Value is YamlSequenceNode)
                    {
                        SetForcedCompilationOrder(project, ((YamlSequenceNode)pair.Value).Children);             
                    }
                    else
                    {
                        TryAddParameters(suite, project, pair.Key, pair.Value);
                    }
                }
            }

            // Adding post processors
            SetProjectPostProcessors(suite, project, projectNode);
        }

        private void SetProjectPostProcessors(Suite suite, IPostProcessorsHolder project, YamlNode postProcessorDefinitions)
        {
            Contract.Requires(suite != null);
            Contract.Requires(project != null);
            Contract.Requires(postProcessorDefinitions != null);

            foreach (var pair in parser.EnumerateNamedNodesOf(postProcessorDefinitions, "postprocessors"))
            {
                var name = pair.Key;
                var value = pair.Value;

                PostProcessorId ppid;
                if (value == null)
                {
                    ppid = name;
                }
                else if (value is YamlMappingNode)
                {
                    ppid = parser.GetScalarValue(value, "type", "Post processor type is not specified");
                }
                else
                {
                    ppid = null;
                }

                if (ppid != null)
                {
                    var loader = parametersLoaders.FirstOrDefault(l => l.Supports(ppid.AsString));
                    
                    IProjectParameters param = null;
                    if (loader != null && value != null)
                    {
                        param = loader.Load(suite, ppid.AsString, value, parser);                        
                    }

                    project.AddPostProcessor(new PostProcessDefinition(name, ppid)
                    {
                        Parameters = param
                    });
                }
            }
        }

        private void SetProjectReferences(Project project, IEnumerable<YamlNode> referenceNodes)
        {
            Contract.Requires(project != null);
            Contract.Requires(referenceNodes != null);

            foreach (var referenceNode in referenceNodes)
            {
                foreach (var reference in referenceLoader.LoadReference(referenceNode))
                    project.AddReference(reference);
            }
        }
        
        private void SetForcedCompilationOrder(Project project, IEnumerable<YamlNode> builderNames)
        {
            Contract.Requires(project != null);
            Contract.Requires(builderNames != null);
            
            foreach (var builderNameNode in builderNames.OfType<YamlScalarNode>())
            {
                var relativeBuilderName = builderNameNode.Value;
                project.AddOrderedCompilationStep(relativeBuilderName);
            }
        }

        private void SetProjectType(Project project, string typeString)
        {
            Contract.Requires(project != null);
            Contract.Requires(typeString != null);

            switch (typeString.ToLowerInvariant())
            {
                case "executable":
                    project.Type = ProjectType.Executable;
                    break;
                case "windows-executable":
                    project.Type = ProjectType.WindowsExecutable;
                    break;
                case "static-library":
                case "staticlibrary":
                case "static":
                    project.Type = ProjectType.StaticLibrary;
                    break;
                default:
                    project.Type = ProjectType.Library;
                    break;
            }
        }

        private void TryAddParameters(Suite suite, IProjectParametersHolder target, YamlNode key, YamlNode value)
        {
            if (parametersLoaders != null)
            {
                var scalarKey = key as YamlScalarNode;

                if (scalarKey != null)
                {
                    string name = scalarKey.Value;
                    var loader = parametersLoaders.FirstOrDefault(l => l.Supports(name));
                    if (loader != null)
                    {
                        var param = loader.Load(suite, name, value, parser);
                        target.AddParameters(name, param);
                    }
                }
            }
        }

        private string ParseVersion(string version)
        {
            if (version != null)
            {
                var result = new StringBuilder(version);
                EnvironmentVariables.ResolveEnvironmentVariables(versioningEnvironmentVariableContext, result);

                return result.ToString();
            }
            else
            {
                return null;
            }
        }
    }
}