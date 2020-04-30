using System.Drawing;

namespace App1
{
	public class ColorConvertor
	{
		public Color Convert(object value)
		{
			var color = Color.White;
			var colorStr = value.ToString().ToLower();
			switch(colorStr)
			{
				case "red":
					color = Color.Red;
					break;
				case "yellow":
					color = Color.Yellow;
					break;
				case "green":
					color = Color.Green;
					break;
				case "blue":
					color = Color.Blue;
					break;
			}

			return color;
		}
	}
}