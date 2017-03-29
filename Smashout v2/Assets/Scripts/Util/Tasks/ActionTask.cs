using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ActionTask : Task {
	private readonly Action action;

	public ActionTask(Action act){
		action = act;
	}

	protected override void Init(){
		SetStatus (TaskStatus.Success);
		action ();
	}
}
