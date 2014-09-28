$pwd = get-location
$xsl = "$pwd\Gallio2NUnit.xslt"
$xml = new-object System.Xml.XmlTextReader("$pwd\..\target\test-report.xml")
$xslt = new-object System.Xml.Xsl.XslCompiledTransform
$arglist = new-object System.Xml.Xsl.XsltArgumentList
$output = new-object System.IO.FileStream "$pwd\..\target\test-report-nunit.xml", 'Create'
$xslt.Load($xsl)
$xslt.Transform($xml, $arglist, $output)