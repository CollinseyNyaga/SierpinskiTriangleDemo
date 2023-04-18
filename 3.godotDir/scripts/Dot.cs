using Godot;
using System;

public partial class Dot : Control
{
	// Variables : 
	[Export]
	Color DrawingColor;
	
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	// Called to draw the dot on the control.
	public override void _Draw()
	{
		DrawDot(new Vector2(0,0));
	}
	
	void DrawDot(Vector2 pos)
	{
		// draws a circle at the control's position : 
		DrawArc(center: pos, radius: 1.5f , startAngle: 0, endAngle: 360 , pointCount: 16, color: DrawingColor);
		
		// GD.Print(pos);
	}
	
	public void PositionSelf(Vector2 pos)
	{
		// positions itself at the passed position : 
		this.Position = pos;

	}
}
