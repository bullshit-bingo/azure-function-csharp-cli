#r "CamundaClient.dll"

using System.Net;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;

using CamundaClient;
using CamundaClient.Dto;


public static HttpResponseMessage Run(HttpRequestMessage req, out string outputEventHubMessage, TraceWriter log)
{
    log.Info("Start workflow on camunda instance at: "  + ConfigurationManager.AppSettings["camundaEndpoint"] + "engine/default/");
    var camunda = new CamundaEngineClient(
        new System.Uri(ConfigurationManager.AppSettings["camundaEndpoint"] + "/engine/default/"), 
        null, 
        null);

    string processInstanceId = camunda.BpmnWorkflowService.StartProcessInstance("ticket", new Dictionary<string, object>()
                    {
                        {"payload", "{}" }
                    });

    outputEventHubMessage = "started: " + processInstanceId;

    return req.CreateResponse(HttpStatusCode.OK, "Started " + processInstanceId);
}