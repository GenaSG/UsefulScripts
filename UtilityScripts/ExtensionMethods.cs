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
		float a = Vector3.Distance(Middle.transform.position,End.transform.position);
		float b = Vector3.Distance(Start.transform.position,Target.position);
		float c = Vector3.Distance(Start.transform.position,Middle.transform.position);
		Vector3 CurrentDirection;
		Vector3 NeededDirection;
		if(b<a + c){
			float MiddleAngle = Mathf.Acos((Mathf.Pow(a,2)+Mathf.Pow(c,2)-Mathf.Pow(b,2))/(2*a*c)) * Mathf.Rad2Deg;
			float CurrentMiddleAngle = Vector3.Angle((Start.transform.position-Middle.transform.position),(End.transform.position - Middle.transform.position));
			Vector3 Axis = Middle.transform.InverseTransformDirection(Vector3.Cross((Start.transform.position-Middle.transform.position),(End.transform.position - Middle.transform.position)));
			float DeltaAngle = MiddleAngle - CurrentMiddleAngle;
			Middle.transform.rotation *= Quaternion.AngleAxis(DeltaAngle,Axis);
			CurrentDirection = Start.transform.InverseTransformDirection(End.transform.position - Start.transform.position);
			NeededDirection = Start.transform.InverseTransformDirection(Target.position - Start.transform.position);
			Start.transform.localRotation *= Quaternion.FromToRotation(CurrentDirection,NeededDirection);
		}else{
			CurrentDirection = Start.transform.InverseTransformDirection(Middle.transform.position - Start.transform.position);
			NeededDirection = Start.transform.InverseTransformDirection(Target.position - Start.transform.position);
			Start.transform.localRotation *= Quaternion.FromToRotation(CurrentDirection,NeededDirection);
			CurrentDirection = Middle.transform.InverseTransformDirection(End.transform.position - Middle.transform.position);
			NeededDirection = Middle.transform.InverseTransformDirection(Target.position - Middle.transform.position);
			Middle.transform.localRotation *= Quaternion.FromToRotation(CurrentDirection,NeededDirection);
		}
		
	}

	public static void ik(Transform End,Vector3 TargetPosition){
		Transform Middle = End.parent;
		Transform Start = Middle.parent;

		float a = Vector3.Distance(Middle.transform.position,End.transform.position);
		float b = Vector3.Distance(Start.transform.position,TargetPosition);
		float c = Vector3.Distance(Start.transform.position,Middle.transform.position);
		Vector3 CurrentDirection;
		Vector3 NeededDirection;
		if(b<a + c){
			float MiddleAngle = Mathf.Acos((Mathf.Pow(a,2)+Mathf.Pow(c,2)-Mathf.Pow(b,2))/(2*a*c)) * Mathf.Rad2Deg;
			float CurrentMiddleAngle = Vector3.Angle((Start.transform.position-Middle.transform.position),(End.transform.position - Middle.transform.position));
			Vector3 Axis = Middle.transform.InverseTransformDirection(Vector3.Cross((Start.transform.position-Middle.transform.position),(End.transform.position - Middle.transform.position)));
			float DeltaAngle = MiddleAngle - CurrentMiddleAngle;
			Middle.transform.rotation *= Quaternion.AngleAxis(DeltaAngle,Axis);
			CurrentDirection = Start.transform.InverseTransformDirection(End.transform.position - Start.transform.position);
			NeededDirection = Start.transform.InverseTransformDirection(TargetPosition - Start.transform.position);
			Start.transform.localRotation *= Quaternion.FromToRotation(CurrentDirection,NeededDirection);
		}else{
			CurrentDirection = Start.transform.InverseTransformDirection(Middle.transform.position - Start.transform.position);
			NeededDirection = Start.transform.InverseTransformDirection(TargetPosition - Start.transform.position);
			Start.transform.localRotation *= Quaternion.FromToRotation(CurrentDirection,NeededDirection);
			CurrentDirection = Middle.transform.InverseTransformDirection(End.transform.position - Middle.transform.position);
			NeededDirection = Middle.transform.InverseTransformDirection(TargetPosition - Middle.transform.position);
			Middle.transform.localRotation *= Quaternion.FromToRotation(CurrentDirection,NeededDirection);
		}
		
	}

	public static void ik(Transform End,Vector3 TargetPosition, Quaternion TargetRotation){
		Transform Middle = End.parent;
		Transform Start = Middle.parent;
		
		float a = Vector3.Distance(Middle.transform.position,End.transform.position);
		float b = Vector3.Distance(Start.transform.position,TargetPosition);
		float c = Vector3.Distance(Start.transform.position,Middle.transform.position);
		Quaternion deltaRotation = Quaternion.Inverse (End.rotation) * TargetRotation;
		Start.localRotation *= Quaternion.Slerp(Quaternion.identity,deltaRotation,0.33f);
		Middle.localRotation *= Quaternion.Slerp(Quaternion.identity,deltaRotation,0.33f);
		Vector3 CurrentDirection;
		Vector3 NeededDirection;
		if(b<a + c){
			float MiddleAngle = Mathf.Acos((Mathf.Pow(a,2)+Mathf.Pow(c,2)-Mathf.Pow(b,2))/(2*a*c)) * Mathf.Rad2Deg;
			float CurrentMiddleAngle = Vector3.Angle((Start.transform.position-Middle.transform.position),(End.transform.position - Middle.transform.position));
			Vector3 Axis = Middle.transform.InverseTransformDirection(Vector3.Cross((Start.transform.position-Middle.transform.position),(End.transform.position - Middle.transform.position)));
			float DeltaAngle = MiddleAngle - CurrentMiddleAngle;
			Middle.transform.rotation *= Quaternion.AngleAxis(DeltaAngle,Axis);
			CurrentDirection = Start.transform.InverseTransformDirection(End.transform.position - Start.transform.position);
			NeededDirection = Start.transform.InverseTransformDirection(TargetPosition - Start.transform.position);
			Start.transform.localRotation *= Quaternion.FromToRotation(CurrentDirection,NeededDirection);
		}else{
			CurrentDirection = Start.transform.InverseTransformDirection(Middle.transform.position - Start.transform.position);
			NeededDirection = Start.transform.InverseTransformDirection(TargetPosition - Start.transform.position);
			Start.transform.localRotation *= Quaternion.FromToRotation(CurrentDirection,NeededDirection);
			CurrentDirection = Middle.transform.InverseTransformDirection(End.transform.position - Middle.transform.position);
			NeededDirection = Middle.transform.InverseTransformDirection(TargetPosition - Middle.transform.position);
			Middle.transform.localRotation *= Quaternion.FromToRotation(CurrentDirection,NeededDirection);
		}

		End.rotation = TargetRotation;

	}

	public static void ik(Transform End,Vector3 TargetPosition, Quaternion TargetRotation,float positionsWeight,float rotationWeight){
		Transform Middle = End.parent;
		Transform Start = Middle.parent;

		TargetPosition = Vector3.Lerp (End.position,TargetPosition,positionsWeight);
		TargetRotation = Quaternion.Slerp (End.rotation,TargetRotation,rotationWeight);

		float a = Vector3.Distance(Middle.transform.position,End.transform.position);
		float b = Vector3.Distance(Start.transform.position,TargetPosition);
		float c = Vector3.Distance(Start.transform.position,Middle.transform.position);
		Quaternion deltaRotation = Quaternion.Inverse (End.rotation) * TargetRotation;
		Start.localRotation *= Quaternion.Slerp(Quaternion.identity,deltaRotation,0.33f);
		Middle.localRotation *= Quaternion.Slerp(Quaternion.identity,deltaRotation,0.33f);
		Vector3 CurrentDirection;
		Vector3 NeededDirection;
		if(b<a + c){
			float MiddleAngle = Mathf.Acos((Mathf.Pow(a,2)+Mathf.Pow(c,2)-Mathf.Pow(b,2))/(2*a*c)) * Mathf.Rad2Deg;
			float CurrentMiddleAngle = Vector3.Angle((Start.transform.position-Middle.transform.position),(End.transform.position - Middle.transform.position));
			Vector3 Axis = Middle.transform.InverseTransformDirection(Vector3.Cross((Start.transform.position-Middle.transform.position),(End.transform.position - Middle.transform.position)));
			float DeltaAngle = MiddleAngle - CurrentMiddleAngle;
			Middle.transform.rotation *= Quaternion.AngleAxis(DeltaAngle,Axis);
			CurrentDirection = Start.transform.InverseTransformDirection(End.transform.position - Start.transform.position);
			NeededDirection = Start.transform.InverseTransformDirection(TargetPosition - Start.transform.position);
			Start.transform.localRotation *= Quaternion.FromToRotation(CurrentDirection,NeededDirection);
		}else{
			CurrentDirection = Start.transform.InverseTransformDirection(Middle.transform.position - Start.transform.position);
			NeededDirection = Start.transform.InverseTransformDirection(TargetPosition - Start.transform.position);
			Start.transform.localRotation *= Quaternion.FromToRotation(CurrentDirection,NeededDirection);
			CurrentDirection = Middle.transform.InverseTransformDirection(End.transform.position - Middle.transform.position);
			NeededDirection = Middle.transform.InverseTransformDirection(TargetPosition - Middle.transform.position);
			Middle.transform.localRotation *= Quaternion.FromToRotation(CurrentDirection,NeededDirection);
		}
		
		End.rotation = TargetRotation;
		
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
