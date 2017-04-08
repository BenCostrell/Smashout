using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Services {
	public static EventManager EventManager { get; set; }
	public static GameManager GameManager { get; set; }
	public static PrefabDB PrefabDB { get; set; }
    public static LevelQueue LevelQueue { get; set; }
	public static TaskManager TaskManager { get; set; }
    public static BlockManager BlockManager { get; set; }
    public static UIManager UIManager { get; set; }
    public static InputManager InputManager { get; set; }
}
