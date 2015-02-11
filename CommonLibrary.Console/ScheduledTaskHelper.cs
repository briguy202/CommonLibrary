using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32.TaskScheduler;
using System.Threading;

namespace CommonLibrary.Console {
	public class ScheduledTaskHelper {
		public static void WaitForRunningTask(string[] taskList, int sleepSeconds, ConsoleHelper helper) {
			string runningTaskName = string.Empty;
			using (TaskService service = new TaskService()) {
				RunningTaskCollection running = service.GetRunningTasks(true);

				foreach (string taskName in taskList) {
					helper.LogFormat(ConsoleHelper.WriteMode.Log, "Checking for running task {0}.", taskName);
					Task task = service.GetTask(taskName);
					if (task == null) {
						throw new ConsoleException("Could not find a scheduled task at the path '{0}'. Be sure to provide the full folder path of the scheduled task, not just the name.", taskName);
					}

					if (running.Any((runningTask) => runningTask.Path == taskName)) {
						runningTaskName = taskName;
						break;
					}
					helper.LogFormat(ConsoleHelper.WriteMode.Log, "Task {0} is not running.", taskName);
				}
			}

			if (!string.IsNullOrEmpty(runningTaskName)) {
				helper.LogFormat(ConsoleHelper.WriteMode.Both, "Waiting for task {0} to complete, sleeping for {1} seconds (waking at {2}) ...", runningTaskName, sleepSeconds.ToString(), DateTime.Now.AddSeconds(sleepSeconds).ToString("h:mm:ss tt"));
				Thread.Sleep(sleepSeconds * 1000);
				helper.LogFormat(ConsoleHelper.WriteMode.Both, "Waking up...");
				ScheduledTaskHelper.WaitForRunningTask(taskList, sleepSeconds, helper);
			}
		}
	}
}
