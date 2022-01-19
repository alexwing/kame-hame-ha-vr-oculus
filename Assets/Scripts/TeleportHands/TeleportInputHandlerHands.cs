/************************************************************************************

See SampleFramework license.txt for license terms.  Unless required by applicable law 
or agreed to in writing, the sample code is provided “AS IS” WITHOUT WARRANTIES OR 
CONDITIONS OF ANY KIND, either express or implied.  See the license for specific 
language governing permissions and limitations under the license.

************************************************************************************/

using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

/// <summary>
/// When this component is enabled, the player will be able to aim and trigger teleport behavior using Oculus Touch controllers.
/// </summary>
public class TeleportInputHandlerHands : TeleportInputHandler
{
	public Transform LeftHand;
	public Transform RightHand;

	public static bool aim = false;
	public static bool teleport = false;


	public void aimClosed()
	{
		aim = false;

	}
	public void aimOpened()
	{
		aim = true;
	}
	public void teleportClosed()
	{
		teleport = false;

	}
	public void teleportOpened()
	{
		teleport = true;
	}

    public void Update()
    {
	
	}

    /// <summary>
    /// Based on the input mode, controller state, and current intention of the teleport controller, return the apparent intention of the user.
    /// </summary>
    /// <returns></returns>
    public override LocomotionTeleport.TeleportIntentions GetIntention()
	{
		if (!isActiveAndEnabled)
		{
			return global::LocomotionTeleport.TeleportIntentions.None;
		}
		
		if (aim)
		{
			Debug.Log("AIM teleport");
			return LocomotionTeleport.TeleportIntentions.Aim;

		}
		if (teleport && LocomotionTeleport.CurrentIntention == LocomotionTeleport.TeleportIntentions.Aim)
		{

				Debug.Log("TELEPORT ");
				aim = false;
				teleport = false;
				return LocomotionTeleport.TeleportIntentions.PreTeleport;
        }
        else
        {
			return global::LocomotionTeleport.TeleportIntentions.None;
		}
		Debug.Log("teleport OFF");
		return global::LocomotionTeleport.TeleportIntentions.None;


	}

    public override void GetAimData(out Ray aimRay)
    {
		Transform t =  RightHand;
		aimRay = new Ray(t.position, -t.right);
	}
}
