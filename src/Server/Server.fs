open System.IO

open Microsoft.Extensions.DependencyInjection
open Saturn

let publicPath = Path.GetFullPath "../Client/public"
let port = 8085us

let configureSerialization (services : IServiceCollection) =
    services.AddSingleton<Giraffe.Serialization.Json.IJsonSerializer>(Thoth.Json.Giraffe.ThothSerializer())

let app = application {
    url ("http://0.0.0.0:" + port.ToString() + "/")
    use_router Api.webApp
    memory_cache
    use_static publicPath
    service_config configureSerialization
    use_gzip
}

run app
