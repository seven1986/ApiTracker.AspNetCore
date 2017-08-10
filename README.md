# ApiTracker.AspNetCore 

[![Build status](https://ci.appveyor.com/api/projects/status/p3dp82wh0t997oww?svg=true)](https://ci.appveyor.com/project/seven1986/apitracker-aspnetcore)

PM> Install-Package ApiTracker.AspNetCore


### appsettings.json 配置
```json
 "ApiTrackerSetting": {
    "ElasticConnection": "",
    "DocumentName": "",
    "TimeOut": 500
  }
```
```html
ElasticConnection: elastic地址，例如 http://localhost:9200
DocumentName:文档名称，默认apitracker
TimeOut:写入超时时间，单位毫秒，默认500
```


### Startup.cs 配置

```csharp
// This method gets called by the runtime. Use this method to add services to the container.
public void ConfigureServices(IServiceCollection services)
{
    // Add framework services.
    services.AddMvc();

    // ...其他

    services.Configure<ApiTrackerSetting>(Configuration.GetSection("ApiTrackerSetting"));
    services.AddScoped<ApiTracker.ApiTracker>();
}
```


### API Controller 配置

```csharp
    [Route("values")]
    [ServiceFilter(typeof(ApiTracker.ApiTracker),IsReusable = true)]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // POST api/values
        [HttpPost]
        [ApiTracker.ApiTrackerOptions(EnableClientIp =false)]
        public void Post([FromBody]JObject value)
        {

        }
    }
```

