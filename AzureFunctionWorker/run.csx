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

public static void Run(string myQueueItem, TraceWriter log)
{
    log.Info($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
    pollForTasks(log);
}

public static void Run(TimerInfo myTimer, TraceWriter log)
{
    if (myTimer.IsPastDue)
    {
        log.Info("Timer is running late!");
    }
    log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
    pollForTasks(log);
}

public static void doTheWork(String payload, TraceWriter log)
{
    log.Info("YEAH - got a todo with payload " + payload);
}

private static int pollingNumberOfTasks = 100;
private static int pollingLockTimeInMs = 5 * 60 * 1000;

// TODO: Get something more useful here
private static string workerId = "worker1";
// TODO: make generic by searching for all topics
private static string topic = "reservation";


public static void pollForTasks(TraceWriter log)
{

    var camunda = new CamundaEngineClient(
        new System.Uri("http://13.93.92.185:8080/engine-rest/engine/default/"), 
        null, 
        null);

    var tasks = camunda.ExternalTaskService.FetchAndLockTasks(workerId, pollingNumberOfTasks, topic, pollingLockTimeInMs, new List<string>() { "payload" });
    tasks.ForEach((externalTask) => {
            doTheWork(externalTask.Variables["payload"], log);
            camunda.ExternalTaskService.Complete(workerId, externalTask.Id);
        });    
}