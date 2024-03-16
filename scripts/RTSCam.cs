using Godot;
using System;

public partial class RTSCam : Node3D
{
	//Camera rotation
	int cameraRotationDirection = 0;
	[Export(PropertyHint.Range, "0,10,.1")] float cameraRotationSpeed = .20f;
	[Export(PropertyHint.Range, "0,20,1")] float cameraBaseRotationSpeed = 6f;
	//Camera Movement
	[Export(PropertyHint.Range, "0,100,1")] public float cameraMoveSpeed = 20.0f;
	//Camera Pan
	[Export(PropertyHint.Range, "0,32,4")] int cameraAutoPanMargin = 16;
	[Export(PropertyHint.Range, "0,20,0.5")] float cameraAutoPanSpeed = 16f;
	//flags
	bool cameraCanAutoPan = true;
	bool cameraCanMoveBase = true;
	bool cameraCanProcess = true;
	bool cameraCanZoom = true;
	bool cameraCanRotate = true;
	bool cameraIsRotatingBase = true;
	//Camera Zoom Parameters
	float cameraZoomDirection = 0;
	[Export(PropertyHint.Range, "0,100,1")] float cameraZoomSpeed = 40f;
	[Export(PropertyHint.Range, "0,100,1")] float cameraZoomMin = 4f;
	[Export(PropertyHint.Range, "0,100,1")] float cameraZoomMax = 25f;
	[Export(PropertyHint.Range, "0.2,0.1")] float camerzZoomSpeedDamp = 0.92f;

	float cameraMove;
	[Export] Node3D cameraSocket;
	[Export] Godot.Camera3D camera;

	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!cameraCanProcess){return;}
		cameraBaseMove(delta);
		cameraZoomUpdate(delta);
		cameraAutoPan(delta);
		cammeraBaseRotate(delta);
	}
	public override void _UnhandledInput(InputEvent @event)
	{
		if(@event.IsAction("zoom_in")){ cameraZoomDirection=-1; }
		else if(@event.IsAction("zoom_out")){ cameraZoomDirection=1;}
		if(@event.IsActionPressed("cam_rot_l")){cameraRotationDirection=1; cameraIsRotatingBase = true;}
		else if(@event.IsActionPressed("cam_rot_r")){cameraRotationDirection=-1; cameraIsRotatingBase = true;}
		else if(@event.IsActionReleased("cam_rot_l") || @event.IsActionReleased("cam_rot_r")){cameraIsRotatingBase = false;}
	}

	//Moves the base of the camera with WASD
	public void cameraBaseMove(double delta){	
		if (!cameraCanMoveBase){return;}
		Vector3 velocityDirection = Vector3.Zero;

		if (Input.IsActionPressed("forward")){velocityDirection -= Transform.Basis.Z;}
		if (Input.IsActionPressed("backward")){velocityDirection += Transform.Basis.Z;}
		if (Input.IsActionPressed("right")){velocityDirection += Transform.Basis.X;}
		if (Input.IsActionPressed("left")){velocityDirection -= Transform.Basis.X;}

		Vector3 position = Position;
		position = velocityDirection.Normalized();
		position.X = position.X *  (float)(delta * cameraMoveSpeed);
		position.Y = position.Y *  (float)(delta * cameraMoveSpeed);
		position.Z = position.Z *  (float)(delta * cameraMoveSpeed);
		Position += position;
		//Position += velocityDirection.Normalized() * delta * cameraMoveSpeed;
		//GD.Print(velocityDirection);
	}

	//Controls the zoom of the camera
	public void cameraZoomUpdate(double delta){
		if (!cameraCanZoom){return;}
		float newZoom = Mathf.Clamp(camera.Position.Z + cameraZoomSpeed * (float)delta * cameraZoomDirection, cameraZoomMin, cameraZoomMax);
		//3 lines ahead. need new variable because position is not a variable aargsgsd camera.Position.Z = newZoom;
		Vector3 camPos = camera.Position;
		camPos.Z = newZoom;
		camera.Position = camPos;
		//note end
		cameraZoomDirection *= camerzZoomSpeedDamp;

	}
	//Rotates the camera base
	public void cammeraBaseRotate(double delta){
		if (!cameraCanRotate || !cameraIsRotatingBase){return;}

		cameraBaseRotateLR(delta, cameraRotationDirection);
	}
	//Rotates the camera base from left to right
	public void cameraBaseRotateLR(double delta, float dir){
		Vector3 rotation = Rotation;
		rotation.Y += dir * (float)delta * cameraRotationSpeed * cameraBaseRotationSpeed;
		Rotation = rotation;
	}
	//Enables Window edge panning
	public void cameraAutoPan(double delta){
		if(!cameraCanAutoPan){return;}
		Viewport viewportCurrent = GetViewport();
		Vector2 panDirection  = new Vector2(-1, -1);
		Rect2I viewportVisibleRect = (Rect2I)viewportCurrent.GetVisibleRect();
		Vector2I viewportSize = viewportVisibleRect.Size;
		Vector2 mousePosition = GetViewport().GetMousePosition();
		float margin = cameraAutoPanMargin;
		float zoomFactorZ = camera.Position.Z * 0.1f;

		//X pan
		if((mousePosition.X < margin) || (mousePosition.X > (viewportSize.X - margin))){

			if (mousePosition.X > (viewportSize.X/2)){
				panDirection.X = 1;
			}
			Translate(new Vector3(panDirection.X * (float)delta *cameraAutoPanSpeed , 0 , 0));
		}
		//Y pan
				if((mousePosition.Y < margin) || (mousePosition.Y > (viewportSize.Y - margin))){

			if (mousePosition.Y > (viewportSize.Y/2)){
				panDirection.Y = 1;
			}
			Translate(new Vector3(0, 0,panDirection.Y * (float)delta *cameraAutoPanSpeed * zoomFactorZ));	
		}
	}
}
