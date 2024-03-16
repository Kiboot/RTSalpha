using Godot;
using System;

public partial class Camera3D : Godot.Camera3D
{
	const double camSpeed = 0.5;
	const double zoomSpeed = 1;

	Camera3D cam;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//Viewport viewPort = GetViewport();
		//var viewPortSize = viewPort.GetVisibleRect().Size;
		var viewportSize = GetViewport().GetVisibleRect().Size;
		var mousePosition = GetViewport().GetMousePosition();
		GD.Print("Mouse Position: " +mousePosition+ " Viewport Size: " +viewportSize);
		cam = this;
		var camPos = cam.Position;
		GD.Print("Mouse Position: " +mousePosition+ " Viewport Size: " +viewportSize+ " Cam Pos: " +camPos+ "Cam Rotation" +cam.Rotation); ;

		if(mousePosition.X < 10){
			camPos.X -= (float)(camSpeed * delta);
		}
		else if(mousePosition.X > viewportSize.X - 10){
			camPos.X += (float)(camSpeed * delta);
		}

		if(mousePosition.Y <10){
			camPos.Y -= (float)(camSpeed * delta);
		}
		else if(mousePosition.Y > viewportSize.Y - 10){
			camPos.Y += (float)(camSpeed * delta);
		}
		if(Input.IsActionJustReleased("m3")){
			RotationDegrees += new Vector3(0, 90, 0);
		}
		if(Input.IsActionJustReleased("mwUp")){
			if(Position.DistanceTo(camPos) > 10){
				camPos.Y -= (float)(zoomSpeed * delta);
			}
		}
		if(Input.IsActionJustReleased("mwDown")){
			if(Position.DistanceTo(camPos) < 50){
				camPos.Y += (float)(zoomSpeed * delta);
			}
		}
	}
}
