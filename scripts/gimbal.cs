using Godot;
using System;

public partial class gimbal : Node3D
{
	public Camera3D camera;
	public Node3D innergimbal;
	// Called when the node enters the scene tree for the first time.
	[Export] double max_zoom = 3.0;
	[Export] double min_zoom = 0.5;
	[Export] double zoom_speed = 0.08;
	double zoom = 1.5;

	[Export] double speed = 0.3;
	[Export] double drag_speed = 0.005;
	[Export] double acceleration = 0.08;
	[Export] double mouse_sensitivty = 0.005;
	Vector3 move;

	public override void _Ready()
	{
		camera = GetNode<Camera3D>("InnerGimbal/Camera");
		innergimbal = GetNode<Node3D>("InnerGimbal");

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector3 targetScale = new Vector3(Vector3.One.X * (float)zoom, Vector3.One.Y * (float)zoom, Vector3.One.Z * (float)zoom);
		Vector3 scale = Scale;
		scale.X = Mathf.Lerp(Scale.X, targetScale.X, (float)zoom_speed);
		scale.Y = Mathf.Lerp(Scale.Y, targetScale.Y, (float)zoom_speed);
		scale.Z = Mathf.Lerp(Scale.Z, targetScale.Z, (float)zoom_speed);
		Scale = scale;
		Vector3 rotation = innergimbal.Rotation;
		rotation.X = Mathf.Clamp(rotation.X, -1.1f, 0.3f);
		innergimbal.Rotation = rotation;
		move_cam(delta);
	}

	public override void _Input(InputEvent @event)
	{	
		try{
			InputEventMouseMotion e = (InputEventMouseMotion) @event;
				if (Input.IsActionPressed("rotate_cam")){
					if (@event is InputEventMouseMotion){
						if(e.Relative.X != 0){
							RotateObjectLocal(Vector3.Up, -e.Relative.X * (float)mouse_sensitivty);
						}
						if(e.Relative.Y != 0){
							var y_rotation =  Mathf.Clamp(-e.Relative.Y, -30, 30);
							innergimbal.RotateObjectLocal(Vector3.Right, y_rotation * (float)mouse_sensitivty);
						}
					}
			}
			if(Input.IsActionPressed("move_cam")){
				if(@event is InputEventMouseMotion){
					move.X -= e.Relative.X *(float)drag_speed;
					move.Z -= e.Relative.Y *(float)drag_speed;
				}
			}
		}catch(Exception){}
		if (@event.IsActionPressed("zoom_in")){
			zoom -= zoom_speed;
			GD.Print("Zoomed in");
		}
		if (@event.IsActionPressed("zoom_out")){
			zoom += zoom_speed;
			GD.Print("Zoomed out");
		}
		zoom =Mathf.Clamp(zoom, min_zoom, max_zoom);
		
		base._Input(@event);
	}
	public void move_cam(double delta){
		if (Input.IsActionPressed("forward")){ move.Z = Mathf.Lerp(move.Z, (float)-speed, (float)acceleration);}
		else if (Input.IsActionPressed("backward")){ move.Z = Mathf.Lerp(move.Z, (float)speed, (float)acceleration);}
		else{ move.Z = Mathf.Lerp(move.Z, 0, (float)acceleration);}

		if (Input.IsActionPressed("left")){ move.X = Mathf.Lerp(move.X, -(float)speed, (float)acceleration);}
		else if (Input.IsActionPressed("right")){ move.X = Mathf.Lerp(move.X, (float)speed, (float)acceleration);}
		else{ move.X = Mathf.Lerp(move.X, 0, (float)acceleration);}

		Vector3 translation = innergimbal.Position;
		Vector3 up =Vector3.Up;
		up.Y = up.Y *(float)zoom;
		translation += move.Rotated(up, innergimbal.Rotation.Y*(float)zoom);
		translation.X = Mathf.Clamp(translation.X, -20, 20);
		translation.Z = Mathf.Clamp(translation.Z, -20, 20);
	}
}
