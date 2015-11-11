# APIology.HAL

![Build Status](https://ci.appveyor.com/api/projects/status/github/IsaacSanch/APIology.HAL?branch=master&svg=true)
[![Nuget Version](https://img.shields.io/nuget/v/Apiology.Hal.svg)](https://www.nuget.org/packages/Apiology.Hal/)

A [HAL](https://github.com/mikekelly/hal_specification) implementation for ASP.NET.

## Features

- Function-chaining paradigm when manually creating HAL models.
- Optional attribute-based construction of HAL models.

## Functions

- `HalModel`: The base model.
- `SetRelativeApiPath`: Sets the relative path for links.
- `AddLinks(...)`: Adds a link to "_links" node of the HAL document.
- `AddEmbeddedCollection(...)`: Adds a collection of values to the "_embeds" node of the HAL document.
- `ToActionResult(...)`: (optional) Translates the HalModel into an `IHttpActionResult`.

## Attributes

- `HalModel(..)`: This attribute contains basic settings for the model, such as the relative path for links.
- `HalIgnore()`: This attribute causes the field to be ignored when creating the HAL document.
- `HalLink(...)`: This attribute tell the field to display as a link in the HAL document instead of a field.
- `HalEmbeddedValues(...)`: This attribute allows embedded data to be represented as links or, using the "expand" query string, display the data embedded into the HAL document.
    *eg. /attribution/23?expand=subAttributions*

## Examples

#### Building models using function chaining

```c#
public class SimpleDto
{
    public int Id { get; set; }
    public string Comment { get; set; }
}

public class controller : ApiController
{
    [HttpGet, Route("simpleton/{Id:int}")]
    public IHttpActionResult GetSimpleton(int Id)
    {
        SimpleDto dto = ...;

        return new HalModel(dto)
            .SetRelativeApiPath("~/api/")
            .AddLinks(...)
            .AddEmbeddedCollection(...)
            .ToActionResult(this);
    }
}
```

#### Building models using attributes

```c#
//
// Here's what the models look like...
//
[HalModel("~/api/")]
public class AttributedDto
{
    [HalLink(HalLink.RelForSelf, "attributions/{value}")]
    public int Id { get; set; }

    [HalLink("simpletons", "simpletons/{value}")]
    public int SimpletonId { get; set; }

    [HalEmbeddedValues()]
    public IEnumerable<SubAttributionDto> SubAttributions { get; set; }
}

[HalModel("~/api/")]
public class SubAttributionDto
{
    [HalLink(HalLink.RelForSelf, "attributions/{value}")]
    public int Id { get; set; }
}

//
// And here's what the API Controller looks like...
//
public class controller : ApiController
{
    [HttpGet, Route("attribution/{Id:int}")]
    public dynamic GetAttribution(int Id)
    {
        var attrib = new AttributedDto { ... };

        // return objects directly, attributes do the rest.
        return attrib;
    }

    // OR...

    [HttpGet, Route("attribution/{Id:int}")]
    public IHttpActionResult GetAttribution(int Id)
    {
        var attrib = new AttributedDto { ... };

        // set the response using ASP.NET's ActionResult
        // and use the object as the return value.
        return Ok(attrib);
    }

    // OR...

    [HttpGet, Route("attribution/{Id:int}")]
    public IHttpActionResult GetAttribution(int Id)
    {
        var attrib = new AttributedDto { ... };

        // wrap the object in a HalModel
        return new HalModel(attrib)
            ...;
    }
}
```

## Credits
This project is a long term fork of https://github.com/visualeyes/halcyon in order to support a different methodology.
