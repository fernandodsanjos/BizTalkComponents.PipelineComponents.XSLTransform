# XSLTransform PipelineComponent
XSLTransform BizTalk PipelineComponent enables you to use regular maps in a pipelineComponent.

## Why would you do this? 
A lot of features are missing from the BizTalk mapper and it has to many limitations, with this project i want to show how the mapper could be extended.

I wanted the pipeline to mimic the mapping experience as much as possible ,i did not want to point ut an xslt file.The component runs the map as the mapper does, so instead of specifying an external xslt file you have to specify full assembly name of the specified map.<br>
You can specify multiple mapps by using a pipe (|). The first match found will be used.

## Features
1. Read context properties<br>
When i started, the feature i wanted the most, was the ability to read context properties from a map.<br>
With XSLTransform you can read context properties with this syntax in xslt<br>

```xslt
<xsl:value-of select="msxsl:Read('MessageID')" />
````
If you only specify ,name, namespace *http://schemas.microsoft.com/BizTalk/2003/system-properties* will be assumed. To specify namespace use the syntax bellow<br>

```xslt
<xsl:value-of select="msxsl:Read('MessageID','http://schemas.microsoft.com/BizTalk/2003/system-properties')" />
````
2. Static parameters
You can specify static parameters in the pipeline that can then the be used in the map.
Specify parameters in the format [name]=[value]. Again specify multiple values by using a pipe (|).

Parameters are specified in the xslt map like this
```xslt
  <xsl:param name="static"/>
  <xsl:output omit-xml-declaration="yes" method="xml" version="1.0" />
```

and then used like bellow anywhere in the map
```xslt
<xsl:value-of select="$static"/>
````
3. Using includes
This was a tricky one ..<br>
I wanted to be able to use includes as sometimes i had code that was used be multiple maps, and one change forced me to change ALL maps.

Includes are specified in the xslt map like this
```xslt
<xsl:include href="test-include.xsl"/>
  <xsl:output omit-xml-declaration="yes" method="xml" version="1.0" />
```
I wanted to enable this wo using external file paths, i solved it by importing the include xslt files as resources in BizTalk and then pick them up as a stream.

So all you have to do is make sure that the include file is imported in the same application as the map.

## Limitations
One limitation is that the features cannot be used in a orchestration.

And yes it means you will have to get your hands dirty and use xslt :-)<br>

## +++
As XSLTransform finds the map by assembly fullname, you can have multiple maps in the send pipeline that has the same root node and namespace, the component will find the map that matches the specified assembly fullname.
