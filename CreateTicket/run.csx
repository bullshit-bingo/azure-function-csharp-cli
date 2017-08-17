#r "CamundaClient.dll"

using System.Net;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using CamundaClient;
using CamundaClient.Dto;


public static HttpResponseMessage Run(HttpRequestMessage req, out string outputEventHubMessage, TraceWriter log)
{

    var camunda = new CamundaEngineClient(
        new System.Uri("http://13.93.92.185:8080/engine-rest/engine/default/"), 
        null, 
        null);

    /*
    
    var model = Path.Combine(context.FunctionDirectory, "");
    var fileParam = new FileParameter(File.ReadAllBytes(model), "ticket.bpmn");

    camunda.RepositoryService.Deploy("ticket", new List<object> { fileParam });
    */

    string processInstanceId = camunda.BpmnWorkflowService.StartProcessInstance("ticket", new Dictionary<string, object>()
                    {
                        {"payload", "{}" }
                    });

    outputEventHubMessage = "started: " + processInstanceId;

    return req.CreateResponse(HttpStatusCode.OK, "Started " + processInstanceId);
}