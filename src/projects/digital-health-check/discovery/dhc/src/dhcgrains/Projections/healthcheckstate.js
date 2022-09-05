fromCategory('healthcheck')
  .foreachStream()
  .when({
    "$init": function(state, ev) {
      return {}
    },
    "HealthCheckAddedDataEvent": function(state, ev) {
        for(var k in state) 
        {
            delete state[k];
        }
        for(var j in ev.body) 
        {
            state[j]=ev.body[j];
        }
    },
    "HealthCheckStartedEvent": function(state, ev) {
        for(var k in state) 
        {
            delete state[k];
        }
        for(var j in ev.body) 
        {
            state[j]=ev.body[j];
        }
    },
    "HealthCheckCompleteEvent": function(state, ev) {
        for(var k in state) 
        {
            delete state[k];
        }
        for(var j in ev.body) 
        {
            state[j]=ev.body[j];
        }
    }
    
  })