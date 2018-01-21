# Sd.Helpers.OData
This library is a small helper for System.Web.OData namespace

# Installation
#### Nuget
```
Install-Package Sd.Helpers.OData -ProjectName MyProject
```
# Case
OData is a nice and flexible protocol, which allows moving some data access logic to ui. 
But the problem here is the security. OData was not designed to be secure unlike from GraphQL which allows writing custom validators for each field (ex https://graphql-dotnet.github.io/learn/#authentication-authorization), OData allows only to ignore specific property from EDM model.
In my case, i wanted to implement conditional access properties.

# Odata concern
According to documentation, it`s possible to invoke odata query manually
``` c-sharp
public IQueryable<Product> Get(ODataQueryOptions opts)
{
    var settings = new ODataValidationSettings()
    {
        // Initialize settings as needed.
        AllowedFunctions = AllowedFunctions.AllMathFunctions
    };

    opts.Validate(settings);

    IQueryable results = opts.ApplyTo(products.AsQueryable());
    return results as IQueryable<Product>;
}
```

The problem here is that this solution will work if you don`t have $select or $expand($select) logic in your queries. Otherwise, OData will wrap object and properties into specific internal classes.

# Solution
I wrote a small helper which allows to convert OData object wrapper to List<T> using reflection.

# Usage
1. Add a module into your application (as a rule app.module.ts)
```c-sharp
  public IEnumerable<PrivateKey> Get(ODataQueryOptions options)
        {
            var settings = new ODataQuerySettings()
            {
                PageSize = 5
            };

            var results = options.ApplyTo(_service.GetAll(), settings);

            var res = Sd.Helpers.OData.Converter.ToList<PrivateKey>(results);

            return res;
        }
```

# GitHub
Please feel free to declare issues or contribute: https://github.com/skynet2/Sd.Helpers.OData/
