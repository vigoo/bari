using System.Xml;
using Bari.Core.Model;

namespace Bari.Plugins.CodeContracts.Model
{
    /// <summary>
    /// Project parameter block containing all the possible parameters for CodeContracts 
    /// </summary>
    public class ContractsProjectParameters: IProjectParameters
    {
        public ContractAssemblyMode AssemblyMode { get; set; }
        public bool EnableRuntimeChecking { get; set; }
        public bool RuntimeOnlyPublicSurface { get; set; }
        public bool RuntimeThrowOnFailure { get; set; }
        public bool RuntimeCallSiteRequires { get; set; }
        public bool RuntimeSkipQuantifiers { get; set; }
        public bool RunCodeAnalysis { get; set; }
        public bool NonNullObligations { get; set; }
        public bool BoundsObligations { get; set; }
        public bool ArithmeticObligations { get; set; }
        public bool EnumObligations { get; set; }
        public bool RedundantAssumptions { get; set; }
        public bool RunInBackground { get; private set; }
        public bool ShowSquigglies { get; set; }
        public bool UseBaseLine { get; set; }
        public bool EmitXMLDocs { get; set; }
        public string CustomRewriterAssembly { get; set; }
        public string CustomRewriterClass { get; set; }
        public string LibPaths { get; set; }
        public string ExtraRewriteOptions { get; set; }
        public string ExtraAnalysisOptions { get; set; }
        public string BaseLineFile { get; set; }
        public bool CacheAnalysisResults { get; private set; }
        public ContractCheckingLevel RuntimeCheckingLevel { get; set; }
        public ContractReferenceMode ReferenceAssembly { get; set; }
        public int AnalysisWarningLevel { get; set; }
        public bool InferRequires { get; set; }
        public bool InferEnsures { get; set; }
        public bool InferObjectInvariants { get; set; }
        public bool SuggestAssumptions { get; set; }
        public bool SuggestRequires { get; set; }
        public bool SuggestEnsures { get; set; }
        public bool SuggestObjectInvariants { get; set; }
        public bool DisjunctiveRequires { get; set; }

        /// <summary>
        /// Creates the parameter block with default values
        /// </summary>
        public ContractsProjectParameters()
        {
            EnableRuntimeChecking = true;
            RunCodeAnalysis = false;

            RunInBackground = false;
            CacheAnalysisResults = false;

            AssemblyMode = ContractAssemblyMode.StandardContractRequires;            
            RuntimeOnlyPublicSurface = false;
            RuntimeThrowOnFailure = true;
            RuntimeCallSiteRequires = false;
            RuntimeSkipQuantifiers = false;
            NonNullObligations = true;
            BoundsObligations = true;
            ArithmeticObligations = true;
            EnumObligations = true;
            RedundantAssumptions = true;
            ShowSquigglies = true;
            UseBaseLine = false;
            EmitXMLDocs = false;

            CustomRewriterAssembly = string.Empty;
            CustomRewriterClass = string.Empty;
            LibPaths = string.Empty;
            ExtraRewriteOptions = string.Empty;
            ExtraAnalysisOptions = string.Empty;
            BaseLineFile = string.Empty;

            RuntimeCheckingLevel = ContractCheckingLevel.Full;
            ReferenceAssembly = ContractReferenceMode.Build;
            AnalysisWarningLevel = 0;
            InferRequires = true;
            InferEnsures = true;
            InferObjectInvariants = true;
            SuggestAssumptions = false;
            SuggestRequires = true;
            SuggestEnsures = true;
            SuggestObjectInvariants = true;
            DisjunctiveRequires = false;
        }

        /// <summary>
        /// Writes out the parameters as .csproj properties
        /// </summary>
        /// <param name="writer">XML writer to generate the .csproj property block</param>
        public void ToCsprojProperties(XmlWriter writer)
        {
            writer.WriteElementString("CodeContractsAssemblyMode", XmlConvert.ToString((int)AssemblyMode));
            writer.WriteElementString("CodeContractsEnableRuntimeChecking", XmlConvert.ToString(EnableRuntimeChecking));
            writer.WriteElementString("CodeContractsRuntimeOnlyPublicSurface", XmlConvert.ToString(RuntimeOnlyPublicSurface));
            writer.WriteElementString("CodeContractsRuntimeThrowOnFailure", XmlConvert.ToString(RuntimeThrowOnFailure));
            writer.WriteElementString("CodeContractsRuntimeCallSiteRequires", XmlConvert.ToString(RuntimeCallSiteRequires));
            writer.WriteElementString("CodeContractsRuntimeSkipQuantifiers", XmlConvert.ToString(RuntimeSkipQuantifiers));
            writer.WriteElementString("CodeContractsRunCodeAnalysis", XmlConvert.ToString(RunCodeAnalysis));
            writer.WriteElementString("CodeContractsNonNullObligations", XmlConvert.ToString(NonNullObligations));
            writer.WriteElementString("CodeContractsBoundsObligations", XmlConvert.ToString(BoundsObligations));
            writer.WriteElementString("CodeContractsArithmeticObligations", XmlConvert.ToString(ArithmeticObligations));
            writer.WriteElementString("CodeContractsEnumObligations", XmlConvert.ToString(EnumObligations));
            writer.WriteElementString("CodeContractsRedundantAssumptions", XmlConvert.ToString(RedundantAssumptions));
            writer.WriteElementString("CodeContractsRunInBackground", XmlConvert.ToString(RunInBackground));
            writer.WriteElementString("CodeContractsShowSquigglies", XmlConvert.ToString(ShowSquigglies));
            writer.WriteElementString("CodeContractsUseBaseLine", XmlConvert.ToString(UseBaseLine));
            writer.WriteElementString("CodeContractsEmitXMLDocs", XmlConvert.ToString(EmitXMLDocs));
            writer.WriteElementString("CodeContractsCustomRewriterAssembly", CustomRewriterAssembly);
            writer.WriteElementString("CodeContractsCustomRewriterClass", CustomRewriterClass);
            writer.WriteElementString("CodeContractsLibPaths", LibPaths);
            writer.WriteElementString("CodeContractsExtraRewriteOptions", ExtraRewriteOptions);
            writer.WriteElementString("CodeContractsExtraAnalysisOptions", ExtraAnalysisOptions);
            writer.WriteElementString("CodeContractsBaseLineFile", BaseLineFile);
            writer.WriteElementString("CodeContractsCacheAnalysisResults", XmlConvert.ToString(CacheAnalysisResults));
            writer.WriteElementString("CodeContractsRuntimeCheckingLevel", RuntimeCheckingLevel.ToString());
            writer.WriteElementString("CodeContractsReferenceAssembly", ReferenceAssembly.ToString());
            writer.WriteElementString("CodeContractsAnalysisWarningLevel", XmlConvert.ToString(AnalysisWarningLevel));
            writer.WriteElementString("CodeContractsInferRequires", XmlConvert.ToString(InferRequires));
            writer.WriteElementString("CodeContractsInferEnsures", XmlConvert.ToString(InferEnsures));
            writer.WriteElementString("CodeContractsInferObjectInvariants", XmlConvert.ToString(InferObjectInvariants));
            writer.WriteElementString("CodeContractsSuggestAssumptions", XmlConvert.ToString(SuggestAssumptions));
            writer.WriteElementString("CodeContractsSuggestRequires", XmlConvert.ToString(SuggestRequires));
            writer.WriteElementString("CodeContractsSuggestEnsures", XmlConvert.ToString(SuggestEnsures));
            writer.WriteElementString("CodeContractsSuggestObjectInvariants", XmlConvert.ToString(SuggestObjectInvariants));
            writer.WriteElementString("CodeContractsDisjunctiveRequires", XmlConvert.ToString(DisjunctiveRequires));
        }
    }
}