using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager {
	private readonly List<Task> tasks = new List<Task>();

	public void AddTask(Task task){
		Debug.Assert (task != null);
		Debug.Assert (!task.IsAttached);
		tasks.Add (task);
		task.SetStatus (Task.TaskStatus.Pending);
	}

	public void Update(){
		Task task;
		for (int i = tasks.Count - 1; i >= 0; i--) {
			task = tasks [i];

			if (task.IsPending) {
				task.SetStatus (Task.TaskStatus.Working);
			}

			if (task.IsFinished) {
				HandleCompletion (task, i);
			} else {
				task.Update ();
				if (task.IsFinished) {
					HandleCompletion (task, i);
				}
			}
		}
	}

	private void HandleCompletion(Task task, int taskIndex){
		if ((task.nextTask != null) && task.IsSuccessful) {
			AddTask (task.nextTask);
		}

		tasks.RemoveAt (taskIndex);
		task.SetStatus (Task.TaskStatus.Detached);
	}

}
