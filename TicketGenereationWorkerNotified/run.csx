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



public static void Run(String myEventHubMessage, TraceWriter log)
{
    log.Info($"C# Timer trigger function executed because of event: {myEventHubMessage}");
    pollForTasks(log);
}

public static void doTheWork(Variable payload, TraceWriter log)
{
    log.Info("YEAH - got a todo with payload " + payload);
}

private static int pollingNumberOfTasks = 100;
private static int pollingLockTimeInMs = 5 * 60 * 1000;

// TODO: Get something more useful here
private static string workerId = "worker1";
// TODO: make generic by searching for all topics
private static string topic = "generate-ticket";


public static void pollForTasks(TraceWriter log)
{

    var camunda = new CamundaEngineClient(
        new System.Uri("http://13.93.92.185:8080/engine-rest/engine/default/"),
        null,
        null);

    var tasks = camunda.ExternalTaskService.FetchAndLockTasks(workerId, pollingNumberOfTasks, topic, pollingLockTimeInMs, new List<string>() { "payload" });

    log.Info($"Polled tasks: {tasks.Count}");

    foreach (var externalTask in tasks)
    {
        log.Info($"Execute task: {externalTask}");
        Variable payload = null;
        if (externalTask.Variables.ContainsKey("payload"))
        {
            payload = externalTask.Variables["payload"];
        }
        doTheWork(payload, log);
        camunda.ExternalTaskService.Complete(workerId, externalTask.Id);
    }
}