Write-Output "Converting Gallio results to Nunit results with root $root"
$root = $args[0]
$xsl = "$root\scripts\Gallio2NUnit.xslt"
$xml = new-object System.Xml.XmlTextReader("$root\target\test-report.xml")
$xslt = new-object System.Xml.Xsl.XslCompiledTransform
$arglist = new-object System.Xml.Xsl.XsltArgumentList
$output = new-object System.IO.FileStream "$root\target\test-report-nunit.xml", 'Create'
$xslt.Load($xsl)
$xslt.Transform($xml, $arglist, $output)