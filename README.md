# XSLTransform PipelineComponent
XSLTransform BizTalk PipelineComponent enables you to use regular maps in a pipelineComponent.

## Why would you do this? 
A lot of features are missing from the BizTalk mapper and it has to many limitations, with this project i want to show how the mapper could be extended.

I wanted the pipeline to mimic the mapping experience as much as possible ,i did not want to point ut an xslt file.The component runs the map as the mapper does, so instead of specifying an external xslt file you have to specify full assembly name of the specified map.<br>
You can specify multiple mapps by using a pipe (|). The first match found will be used.

Make sure to use the _function-available_ as the functions does not exist in VS<br/>
<xsl:if test="function-available('msxsl:Write')">

## Features
1. **Read Context Properties**<br/>
When i started, the feature i wanted the most, was the ability to read context properties from a map.<br>
With XSLTransform you can read context properties with this syntax in xslt<br>

```xslt
<xsl:value-of select="msxsl:Read('MessageID')" />
````
If you only specify ,name, namespace *http://schemas.microsoft.com/BizTalk/2003/system-properties* will be assumed. To specify namespace use the syntax bellow<br>

```xslt
<xsl:value-of select="msxsl:Read('MessageID','http://schemas.microsoft.com/BizTalk/2003/system-properties')" />
````
2. **Writing And Promoting Values To Context**<br/>
Sometimes you have to promote properties in a pipeline. Unfortunally to promote a value you first have to read, at least a part, of the message.<br/>
But if you have to use a map on a port you might as well promote, or even better, write context directly from the map.<p>
Writing and Promoting to Context property is almost the same as reading one. See example bellow.
 
```xslt
 <xsl:if test="function-available('msxsl:Write')">
      <xsl:variable name="WriteInvoiceID" select="msxsl:Write('InvoiceID','http://schemas',ns13:ID/text())"/>
     <xsl:variable name="WriteCompany" select="msxsl:Promote('IsCompany','http://schemas',$Company)"/>
  </xsl:if>
 ````

3. **Static Parameters**<br/>
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
4. **Using Includes**<br/>
This was a tricky one ..<br>
I wanted to be able to use includes as sometimes i had code that was used be multiple maps, and one change forced me to change ALL maps.

Includes are specified in the xslt map like this
```xslt
<xsl:include href="test-include.xsl"/>
  <xsl:output omit-xml-declaration="yes" method="xml" version="1.0" />
```
I wanted to enable this wo using external file paths, i solved it by importing the include xslt files as resources in BizTalk and then pick them up as a stream.

So all you have to do is make sure that the include file is imported in the same application as the map.

5. **Dynamic Map**<br/>
You can use dynamic map usage by specifying a specific map, in the form of fulll qualified name, in a context property called XLSTransform. The component picks the first property with this name.
The default is that a map i required but you can also set the property Map Required to False. Setting the property to false means that if no map is specified the message will simply be passed forward.<br/>
Tip: _You can specify a default map and override it with the context property XLSTransform, as the context property XLSTransform will be added to the top of the list of maps to evaluate_

## Limitations
One limitation is that the component, being a pipeline component, cannot be used in a orchestration.

And yes it means you will have to get your hands dirty and use xslt :-), even though you can use a regular map.<br>

## +++
As XSLTransform finds the map by assembly fullname, you can have multiple maps in the send pipeline that has the same root node and namespace, the component will find the map that matches the specified assembly fullname.
