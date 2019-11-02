using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Events : MonoBehaviour {
    public static Events gInstance;
    public delegate void EventsDelegate(string eventName, params object[] eventParams);
    public event EventsDelegate gEvent;

    private bool _alreadyRaised = false;
    private Queue<Tuple<string, object[]>> _eventQueue = new Queue<Tuple<string, object[]>>();

    // Awake is always called before any Start functions
    void Awake() {
        // Check if instance already exists
        if (gInstance == null) gInstance = this;
        // If instance already exists and it's not this:
        if (gInstance != this)
            throw new ApplicationException("Only one events object.");
    }

    public void RaiseEventImmediate(string eventName, params object[] eventParams) {
        Debug.LogFormat("events: {0} {1}", eventName, gEvent == null);
        if (gEvent == null)
        {
            Debug.LogFormat("we are gEvents? {0}", Events.gInstance == this);
        }
        
        gEvent(eventName, eventParams);
    }

    public void RaiseEvent(string eventName, params object[] eventParams) {
        _eventQueue.Enqueue(Tuple.Create(eventName, eventParams));
        if (!_alreadyRaised) {
            StartCoroutine(RaiseEventInternal());
        }
    }

    private IEnumerator RaiseEventInternal() {
        _alreadyRaised = true;
        yield return null; // defer to next frame
        _alreadyRaised = false;
        Queue<Tuple<string, object[]>> curEventBatch = _eventQueue;
        _eventQueue = new Queue<Tuple<string, object[]>>();
        foreach (Tuple<string, object[]> t in curEventBatch) {
            gEvent(t.Item1, t.Item2);
        }
    }

}
