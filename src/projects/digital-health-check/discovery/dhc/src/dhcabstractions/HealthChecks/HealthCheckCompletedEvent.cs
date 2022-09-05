using System;
namespace dhc;
public class HealthCheckCompletedEvent
{
    public HealthCheckResult HealthCheckResult{get;set;}
    public Guid HealthCheckId{get;set;}
}