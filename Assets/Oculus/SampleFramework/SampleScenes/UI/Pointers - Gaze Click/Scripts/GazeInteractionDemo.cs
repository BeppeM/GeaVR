/************************************************************************************

Copyright   :   Copyright 2017 Oculus VR, LLC. All Rights reserved.

Licensed under the Oculus VR Rift SDK License Version 3.4.1 (the "License");
you may not use the Oculus VR Rift SDK except in compliance with the License,
which is provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at

https://developer.oculus.com/licenses/sdk-3.4.1


Unless required by applicable law or agreed to in writing, the Oculus VR SDK
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

************************************************************************************/

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class GazeInteractionDemo : MonoBehaviour {
    public GazeInteractionInputModule gazeInputModule;
	
    private float newGazeInteractionDelay = 0;

    // Use this for initialization
    void Start()
    {
        FindObjectOfType<OVRPlayerController>().SetSkipMouseRotation(true);
    }


    public void SetNewGazeInteractionDelay(float d)
    {
        newGazeInteractionDelay = d;

    }

    public void ApplyNewGazeInteractionDelay()
    {
        gazeInputModule.SetActivationDwellTime(newGazeInteractionDelay);
    }
}
