module Application.Routes

open Aggregation.Application.Domain
open Giraffe
open Microsoft.AspNetCore.Http

let routes : HttpFunc -> HttpContext -> HttpFuncResult =
    choose [
        GET >=>
            choose [
                route "/" >=> text "Aggregation BC"
            ]
        POST >=>
            choose [
                route "/aggregate-from-providers" >=> AggregateFromProviders.handler
            ]
        setStatusCode 404 >=> text "Not Found" ]