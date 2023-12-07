using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ExileCore;

public class ProcessPicker
{
	public static int? ShowDialogBox(IEnumerable<Process> processes)
	{
		using Form form = new Form
		{
			Text = "Pick PoE process to attach to",
			FormBorderStyle = FormBorderStyle.FixedDialog,
			StartPosition = FormStartPosition.CenterScreen,
			MinimizeBox = false,
			MaximizeBox = false
		};
		Button button = new Button
		{
			Text = "Exit",
			DialogResult = DialogResult.Cancel
		};
		Button button2 = new Button
		{
			Text = "Wait",
			DialogResult = DialogResult.Retry
		};
		List<Button> list = processes.Select((Process p) => new Button
		{
			Text = $"Process #{p.Id} ({p.ProcessName}), started at {p.StartTime.ToLongTimeString()}",
			DialogResult = DialogResult.OK
		}).ToList();
		int num = 10;
		int num2 = num;
		int num3 = 23;
		int num4 = 300;
		button2.SetBounds(num, num2, num4, num3);
		num2 += num3;
		button.SetBounds(num, num2, num4, num3);
		num2 += num3;
		int? selectedProcessIndex = null;
		foreach (var item in list.Select((Button b, int i) => (b, i)))
		{
			var (button3, j) = item;
			button3.SetBounds(num, num2, num4, num3);
			button3.Click += delegate
			{
				selectedProcessIndex = j;
			};
			num2 += num3;
		}
		form.ClientSize = new Size(num4 + num * 2, num2 + num);
		form.Controls.AddRange(new Control[2] { button2, button }.Concat(list).ToArray());
		form.CancelButton = button2;
		if (form.ShowDialog() == DialogResult.Retry)
		{
			return -1;
		}
		return selectedProcessIndex;
	}
}
