using Godot;
using System;

public partial class RTSCam : Node3D
{
	//Camera rotation
	int cameraRotationDirection = 0;
	[Export(PropertyHint.Range, "0,10,.1")] float camRotSpd = .20f;
	[Export(PropertyHint.Range, "0,20,1")] float camBaseRotSpd = 6f;
	[Export(PropertyHint.Range, "0,10,1")] float camSktRotXMin = -1.20f;
	[Export(PropertyHint.Range, "0,10,1")] float camSktRotXMax = -.20f;
	//Camera Movement
	[Export(PropertyHint.Range, "0,100,1")] public float camMvSpd = 20.0f;
	//Camera Pan
	[Export(PropertyHint.Range, "0,32,4")] int camAutoPanMargin = 16;
	[Export(PropertyHint.Range, "0,20,0.5")] float camAutoPanSpd = 16f;
	//flags
	bool camCanAutoPan = true;
	bool camCanMvBase = true;
	bool camCanProcess = true;
	bool camCanZoom = true;
	bool camCanRot = true;

	bool camCanRotSktX = true;
	bool camCanRotByMouseOffset = true;
	//Internal flags
	bool camIsRotBase = false;
	bool camIsRotMouse = false;
	Vector2 mouseLastPos= Vector2.Zero;

	//Camera Zoom Parameters
	float camZoomDir = 0;
	[Export(PropertyHint.Range, "0,100,1")] float camZoomSpd = 40f;
	[Export(PropertyHint.Range, "0,100,1")] float camZoomMin = 4f;
	[Export(PropertyHint.Range, "0,100,1")] float camZoomMax = 25f;
	[Export(PropertyHint.Range, "0.2,0.1")] float camZoomSpdDamp = 0.92f;

	float cameraMove;
	[Export] Node3D camSkt;
	[Export] Godot.Camera3D cam;

	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!camCanProcess){return;}
		cameraBaseMove(delta);
		cameraZoomUpdate(delta);
		cameraAutoPan(delta);
		cameraBaseRotate(delta);
		cameraRotateToMouseOffsets(delta);
	}
	public override void _UnhandledInput(InputEvent @event)
	{
		if(@event.IsAction("zoom_in")){ camZoomDir=-1; }
		else if(@event.IsAction("zoom_out")){ camZoomDir=1;}
		//Camera Rotations
		if(@event.IsActionPressed("cam_rot_l")){cameraRotationDirection=1; camIsRotBase = true;}
		else if(@event.IsActionPressed("cam_rot_r")){cameraRotationDirection=-1; camIsRotBase = true;}
		else if(@event.IsActionReleased("cam_rot_l") || @event.IsActionReleased("cam_rot_r")){camIsRotBase = false;}

		if(@event.IsActionPressed("rotate_cam")){
			mouseLastPos = GetViewport().GetMousePosition();
			camIsRotMouse = true;
		}
		else if(@event.IsActionReleased("rotate_cam")){camIsRotMouse = false;}

	}

	//Moves the base of the camera with WASD
	public void cameraBaseMove(double delta){	
		if (!camCanMvBase){return;}
		Vector3 velocityDirection = Vector3.Zero;

		if (Input.IsActionPressed("forward")){velocityDirection -= Transform.Basis.Z;}
		if (Input.IsActionPressed("backward")){velocityDirection += Transform.Basis.Z;}
		if (Input.IsActionPressed("right")){velocityDirection += Transform.Basis.X;}
		if (Input.IsActionPressed("left")){velocityDirection -= Transform.Basis.X;}

		Vector3 position = Position;
		position = velocityDirection.Normalized();
		position.X = position.X *  (float)(delta * camMvSpd);
		position.Y = position.Y *  (float)(delta * camMvSpd);
		position.Z = position.Z *  (float)(delta * camMvSpd);
		Position += position;
		//Position += velocityDirection.Normalized() * delta * cameraMoveSpeed;
		//GD.Print(velocityDirection);
	}

	//Controls the zoom of the camera
	public void cameraZoomUpdate(double delta){
		if (!camCanZoom){return;}
		float newZoom = Mathf.Clamp(cam.Position.Z + camZoomSpd * (float)delta * camZoomDir, camZoomMin, camZoomMax);
		//3 lines ahead. need new variable because position is not a variable aargsgsd camera.Position.Z = newZoom;
		Vector3 camPos = cam.Position;
		camPos.Z = newZoom;
		cam.Position = camPos;
		//note end
		camZoomDir *= camZoomSpdDamp;

	}
	//Rotates the camera socket based on the mouse offset
	public void cameraRotateToMouseOffsets(double delta){
		if (!camCanRotByMouseOffset || !camIsRotMouse){return;}
		Vector2 mouseOffset = GetViewport().GetMousePosition();
		mouseOffset -= mouseLastPos;
		mouseLastPos = GetViewport().GetMousePosition();
		cameraBaseRotateLR(delta, mouseOffset.X);
		cameraSocketRotateX(delta, mouseOffset.Y);
		
	}
	//Rotates the camera base
	public void cameraBaseRotate(double delta){
		if (!camCanRot || !camIsRotBase){return;}

		cameraBaseRotateLR(delta, cameraRotationDirection * camRotSpd);
	}
	//Rotate the socket of the camera using mouse
	public void cameraSocketRotateX(double delta, float dir){
		if (!camCanRotSktX){return;}
		float newRotationX = camRotSpd;
		newRotationX -= dir * (float)delta * camRotSpd;
		//using try to stamp out errors caused by RotationMin getting to more than RotationMax
		newRotationX = Mathf.Clamp(newRotationX, camSktRotXMin, camSktRotXMax);
			//newRotationX =
		Vector3 cameraSocketRotation = camSkt.Rotation;
		cameraSocketRotation.X = newRotationX;
		camSkt.Rotation = cameraSocketRotation;
	}

	
	//Rotates the camera base from left to right
	public void cameraBaseRotateLR(double delta, float dir){
		Vector3 rotation = Rotation;
		rotation.Y += dir * (float)delta * camRotSpd * camBaseRotSpd;
		Rotation = rotation;
	}
	//Enables Window edge panning
	public void cameraAutoPan(double delta){
		if(!camCanAutoPan){return;}
		Viewport viewportCurrent = GetViewport();
		Vector2 panDirection  = new Vector2(-1, -1);
		Rect2I viewportVisibleRect = (Rect2I)viewportCurrent.GetVisibleRect();
		Vector2I viewportSize = viewportVisibleRect.Size;
		Vector2 mousePosition = GetViewport().GetMousePosition();
		float margin = camAutoPanMargin;
		float zoomFactorZ = cam.Position.Z * 0.1f;

		//X pan
		if((mousePosition.X < margin) || (mousePosition.X > (viewportSize.X - margin))){

			if (mousePosition.X > (viewportSize.X/2)){
				panDirection.X = 1;
			}
			Translate(new Vector3(panDirection.X * (float)delta *camAutoPanSpd , 0 , 0));
		}
		//Y pan
				if((mousePosition.Y < margin) || (mousePosition.Y > (viewportSize.Y - margin))){

			if (mousePosition.Y > (viewportSize.Y/2)){
				panDirection.Y = 1;
			}
			Translate(new Vector3(0, 0,panDirection.Y * (float)delta *camAutoPanSpd * zoomFactorZ));	
		}
	}
}
