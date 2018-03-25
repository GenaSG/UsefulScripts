using UnityEngine;
using System.Collections;

public static class ExtensionMethods
{
	
	public static Quaternion PhysRotate(Quaternion current,Quaternion target,ref Quaternion velocity,float accel,float drag,float timeDelta){
		Vector3 deltaEuler = Vector3.zero;
		deltaEuler.x = Mathf.DeltaAngle (current.eulerAngles.x,target.eulerAngles.x);
		deltaEuler.y = Mathf.DeltaAngle (current.eulerAngles.y,target.eulerAngles.y);
		deltaEuler.z = Mathf.DeltaAngle (current.eulerAngles.z,target.eulerAngles.z);
		Vector3 nextStep = deltaEuler * timeDelta * accel;
		if (nextStep.magnitude > deltaEuler.magnitude) {
			nextStep = deltaEuler;
		}
		velocity *= Quaternion.Euler(nextStep * timeDelta);
		velocity = Quaternion.Slerp (velocity, Quaternion.identity,drag*timeDelta);
		return current * velocity;
	}

	public static Vector3 PhysMove(Vector3 current,Vector3 target,ref Vector3 velocity,float accel,float drag,float timeDelta){
		Vector3 delta = target - current;
		Vector3 nextStep = delta * timeDelta * accel;
		if (nextStep.magnitude > delta.magnitude) {
			nextStep = delta;
		}
		velocity += nextStep;
		velocity = Vector3.Slerp (velocity, Vector3.zero, drag * timeDelta);
		return current + velocity;
	}

	public static Quaternion SmoothRotate(Quaternion current, Quaternion target,ref Vector3 velocity,float speedCoef,float acceleration, float timeDelta){
		Vector3 deltaEuler = Vector3.zero;
		deltaEuler.x = Mathf.DeltaAngle (current.eulerAngles.x,target.eulerAngles.x);
		deltaEuler.y = Mathf.DeltaAngle (current.eulerAngles.y,target.eulerAngles.y);
		deltaEuler.z = Mathf.DeltaAngle (current.eulerAngles.z,target.eulerAngles.z);
		
		velocity = Vector3.Lerp (velocity, deltaEuler,acceleration * timeDelta);
		current *= Quaternion.Euler (velocity * speedCoef * timeDelta);
		return current;
	}

	public static Vector3 SmoothMove(Vector3 current,Vector3 target, ref Vector3 velocity,float speedCoef,float acceleration, float timeDelta){
		Vector3 delta = target - current;
		velocity = Vector3.Lerp (velocity, delta,acceleration * timeDelta);
		current += velocity * speedCoef * timeDelta;
		return current;
	}

	public static void ik(Transform Start,Transform Middle,Transform End,Transform Target){
		ActualIK(End,Middle,Start,Target.position);
		
	}

	public static void ik(Transform End,Vector3 TargetPosition){
		Transform Middle = End.parent;
		Transform Start = Middle.parent;

		ActualIK(End,Middle,Start,TargetPosition);
		
	}

	public static void ik(Transform End,Vector3 TargetPosition, Quaternion TargetRotation){
		Transform Middle = End.parent;
		Transform Start = Middle.parent;
		
		ActualIK(End,Middle,Start,TargetPosition);
		ikRotations (End,Middle,Start,TargetRotation);

	}

	public static void ik(Transform Start,Transform Middle,Transform End,Transform Target,float positionsWeight,float rotationWeight){

		Vector3 TargetPosition = Vector3.Lerp (End.position,Target.position,positionsWeight);
		Quaternion TargetRotation = Quaternion.Slerp (End.rotation,Target.rotation,rotationWeight);

		ActualIK(End,Middle,Start,TargetPosition);
		ikRotations (End,Middle,Start,TargetRotation);
	}

	private static void ikRotations (Transform End,Transform Middle,Transform Start,Quaternion TargetRotation){
		Vector3 rotationAxis = Start.InverseTransformDirection((End.position - Start.position).normalized);
		Quaternion deltaRotation = Quaternion.Inverse (End.rotation) * TargetRotation;
		Vector3 angles;
		angles.x = Mathf.LerpAngle (0, deltaRotation.eulerAngles.x, Mathf.Abs (rotationAxis.x));
		angles.y = Mathf.LerpAngle (0, deltaRotation.eulerAngles.y, Mathf.Abs (rotationAxis.y));
		angles.z = Mathf.LerpAngle (0, deltaRotation.eulerAngles.z, Mathf.Abs (rotationAxis.z));

		Start.rotation *= Quaternion.Slerp(Quaternion.identity,Quaternion.Euler(angles),0.33f);

		rotationAxis = Middle.InverseTransformDirection((End.position - Middle.position).normalized);
		angles.x = Mathf.LerpAngle (0, deltaRotation.eulerAngles.x, Mathf.Abs (rotationAxis.x));
		angles.y = Mathf.LerpAngle (0, deltaRotation.eulerAngles.y, Mathf.Abs (rotationAxis.y));
		angles.z = Mathf.LerpAngle (0, deltaRotation.eulerAngles.z, Mathf.Abs (rotationAxis.z));


		Middle.rotation *= Quaternion.Slerp(Quaternion.identity,Quaternion.Euler(angles),0.33f);
		End.rotation = TargetRotation;
	}

	public static void ik(Transform End,Vector3 TargetPosition, Quaternion TargetRotation,float positionsWeight,float rotationWeight){
		Transform Middle = End.parent;
		Transform Start = Middle.parent;

		TargetPosition = Vector3.Lerp (End.position,TargetPosition,positionsWeight);
		TargetRotation = Quaternion.Slerp (End.rotation,TargetRotation,rotationWeight);

		ActualIK(End,Middle,Start,TargetPosition);
		ikRotations (End,Middle,Start,TargetRotation);
		
	}

	static void ActualIK(Transform End,Transform Middle,Transform Start, Vector3 TargetPosition){

		float a = Vector3.Distance (Middle.position,End.position);//End.localPosition.magnitude;
		float b = Vector3.Distance(Start.position,TargetPosition);//Start.InverseTransformPoint(TargetPosition).magnitude;
		float c = Vector3.Distance(Start.position,Middle.position);//Middle.localPosition.magnitude;
		b = Mathf.Clamp(b,0.0001f,(a + c) - 0.0001f);
		float middleAngle = Mathf.Acos(Mathf.Clamp((Mathf.Pow(a,2)+Mathf.Pow(c,2)-Mathf.Pow(b,2))/(2*a*c),-1,1)) * Mathf.Rad2Deg;
		Vector3 middleToStartDir = Middle.InverseTransformPoint (Start.position).normalized;
		Vector3 middleToEndDir = Middle.InverseTransformPoint (End.position).normalized;
		Vector3 middleAxis = Vector3.Cross (-middleToStartDir,middleToEndDir).normalized;
		Middle.Rotate (middleAxis, Vector3.Angle (middleToStartDir, middleToEndDir) - middleAngle);
		//Middle.localRotation *= Quaternion.AngleAxis (Vector3.Angle (middleToStartDir, middleToEndDir) - middleAngle,middleAxis);

		Vector3 startToEndDir = Start.InverseTransformPoint (End.position).normalized;
		Vector3 startToTargetDir = Start.InverseTransformPoint (TargetPosition).normalized;

		Vector3 startAxis = Vector3.Cross (startToEndDir,startToTargetDir).normalized;

		Start.Rotate (startAxis,Vector3.Angle (startToEndDir, startToTargetDir));
		//Start.rotation *= Quaternion.FromToRotation (startToEndDir,startToTargetDir);
		//Start.localRotation *= Quaternion.AngleAxis (Vector3.Angle (startToEndDir, startToTargetDir), startAxis);

	}

	public static int BoolToInt(bool input){
		if (input) {
			return 1;
		} else {
			return 0;
		}
	}

	public static bool ToggleOrHold(bool input,bool current,ref bool last,ref float holdTime,float switchTime,float timeDelta){
		if (holdTime < switchTime) {
			//Toggle part
			if(input && input != last){
				current = !current;
			}
			last = input;
		} else {
			//Hold part
			if(!input){
				current = false;
			}
		}
		holdTime += timeDelta;
		holdTime *=BoolToInt (input);
		holdTime = Mathf.Clamp (holdTime, 0, switchTime);
		return current;
	}

	public static bool ToggleOrHold(bool input,bool current,ref bool last,ref bool isHolding,ref float holdTime,float switchTime,float timeDelta){
		if (holdTime < switchTime) {
			//Toggle part
			if(!input && input != last && !isHolding){
				current = !current;
			}
			last = input;
			isHolding = false;
		} else {
			isHolding = true;
		}
		holdTime += timeDelta;
		holdTime *=BoolToInt (input);
		holdTime = Mathf.Clamp (holdTime, 0, switchTime);
		return current;
	}

}
