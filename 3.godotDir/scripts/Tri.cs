using Godot;
using System;
using System.Collections.Generic;

public partial class Tri : Control
{
	// Variables :
	
	// triangle edgeSize :

	[Export]
	float EdgeSize;					// number of pixels which is the length of one side of the triangle : 
	
	// triangle points : 
	Vector2 PointA;
	Vector2 PointB;
	Vector2 PointC;
	
	Vector2 CurrentPoint;			// can be any point in 2d space.
	
	float Height;
	
	//
	[Export]
	Color DrawingColor;
	
	//
	Control LabelA;
	Control LabelB;
	Control LabelC;
	
	
	// 
	List<Vector2> SierpinskiPoints = new List<Vector2>();
	
	// 
	Timer PointsDrawTimer;
	float PointsDrawTimeIntervals = 0.001f;
	int currentDrawnPoint = 0;
	
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		LabelA = (Control)GetNode("pointLabels/LabelA");
		LabelB = (Control)GetNode("pointLabels/LabelB");
		LabelC = (Control)GetNode("pointLabels/LabelC");
		
		
		PointsDrawTimer = (Timer)GetNode("PointsDrawTimer");
		PointsDrawTimer.WaitTime = PointsDrawTimeIntervals;
		// points draw timer is started after all the points have been generated : 
	

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
	
	// Called to draw : 
	public override void _Draw()
	{
		// draw the triangle : 
		DrawTriangle();
		
			
		// calculate the sierpinski points and store them in a list : 
		GenerateSierpinskiPoints(5000);
		
		// after all the sierpinski points have been calculated , start the draw timer for each points : 
		PointsDrawTimer.Start();
	}
	
	void DrawTriangle()
	{
		/*
		 Points of the triangle are labeled below : 
					.c
		 
		 
				a.		.b
		*/
		
		
		// calculate the triangle height using the half equilateral triangle dimensions :
		float triangleHeightSquared = (EdgeSize * EdgeSize) - ((EdgeSize/2) * (EdgeSize/2));
		float triangleHeight = System.MathF.Sqrt(triangleHeightSquared);
		// GD.Print(triangleHeight);
		
		
		//	calculate the starting point of drawing on the current window : 
		//	the starting point should be centered : 
		Vector2 startingPoint = GetWindowCenterPoint();
		startingPoint.X = startingPoint.X - EdgeSize/2;
		startingPoint.Y = startingPoint.Y + triangleHeight/2;
		
		
		// draw the base line a to b: 
		Vector2 a = startingPoint;
		
		Vector2 b;
		b.X = a.X + EdgeSize;
		b.Y = a.Y;
		
		DrawLine(from: a , to: b , color: DrawingColor);
		
		// draw line from b to c : 
		Vector2 c = new Vector2();
		c.X = b.X - (EdgeSize/2);
		c.Y = a.Y - triangleHeight;
		
		DrawLine(from: b , to: c , color: DrawingColor);
		
		// draw line from c to a : 
		DrawLine(from: c , to: a , color: DrawingColor);
		
		
		// position labels of the triangle points on the screen : 
		LabelA.Position = a;
		LabelB.Position = b;
		LabelC.Position = c + new Vector2(0,- 30);
		
		
		// set the positions of the triangle : 
		PointA = a;
		PointB = b;
		PointC = c;
		
		// set the height of the triangle : 
		Height = triangleHeight;
		

	}
	
	Vector2 GetWindowCenterPoint()
	{
		// returns the point that the drawing should start from (which is the center of the screen , depending on the screen size) : 
		Vector2 windowSize = DisplayServer.WindowGetSize();
		
		Vector2 center = windowSize;
		center.X = center.X/2;
		center.Y = center.Y/2;
		
		return center;
	}
	
	void GenerateSierpinskiPoints(int points)
	{
		/*
			how the sierpinski's triangle works : 
			
			First draw a triangle , second chose a point x within the triangle as the starting point ; choose a point among a , b , c.
			Draw the midpoint of the line x to the random point among a , b or c.
			From the midpoint you obtain , choose a random point between a , b or c and continue drawing their midpoints.
			
		*/
		
		for(int i = 0; i < points ; i++)
		{
			if(i == points)
			{
				// if its the first point selection , get random point in the triangle as the starting point :
				Vector2 firstPoint = GetRandom2DPointInTriangle();
				
				// get the midpoint of the random point and a random point between A,B and C.
				Vector2 firstMidPoint = (firstPoint/2 + GetRandomAbcPoint()/2);
				
				// set the current point : 
				CurrentPoint = firstMidPoint;
				
				// add the sierpinski point into the array : 
				SierpinskiPoints.Insert(index: i , item: CurrentPoint);
	
				
				// move to next iteration : 
				continue;
			}
			
			// get midpoint of the current point to the random point between a , b and c.
			Vector2 midPoint =  CurrentPoint/2 + GetRandomAbcPoint()/2;
			CurrentPoint = midPoint;

			// add the sierpinski point into the array : 
			SierpinskiPoints.Insert(index: i , item: CurrentPoint);

		}

		
	}
	
	
	Vector2 GetRandom2DPointInTriangle()
	{
		// returns a random 2d point in the triangle : 
		RandomNumberGenerator rng = new RandomNumberGenerator();
		
		Vector2 randPoint;
		randPoint.X = rng.RandfRange(from: PointA.X ,to: PointB.X);
		randPoint.Y = rng.RandfRange(from: PointA.Y ,to: (PointA.Y - Height));
		
		return randPoint;
	}
	
	Vector2 GetRandomAbcPoint()
	{
		// returns a random point between the a , b or c points : 
		Random rn = new Random();
		int randomInt = rn.Next(1, 4);
		
		switch(randomInt)
		{
			case 1: 
				return PointA;
				break;
				
			case 2: 
				return PointB;
				break;
				
			case 3: 
				return PointC;
				break;
				
			default: 
				return PointA;
				break;
		}

	}
	
	public void PointsDrawTimerTimedOut()
	{
		// this method is called when the draw timer times out.
		// after calling the draw methods of canvas item , finally the draw function is recalled from here : 

		if(currentDrawnPoint < SierpinskiPoints.Count)
		{
			
			if(SierpinskiPoints[currentDrawnPoint] != null)
			{
				// draw the dot at the current drawn point index :
				Vector2 pointToDraw = SierpinskiPoints[currentDrawnPoint];
				
				
				// instance of the dot scene : 
				PackedScene dotScene = (PackedScene)ResourceLoader.Load("res://dot.tscn");
				Node dotNode = dotScene.Instantiate();
				
				// add the dot as child of the triangle : 
				this.AddChild(dotNode);
				
				// position the dot on the point to draw position :
				dotNode.Call(method: "PositionSelf" , pointToDraw);			
						
				// set the index of the next point that will be drawn
				currentDrawnPoint = currentDrawnPoint + 1;
			}
		}

	}
	
	

}
