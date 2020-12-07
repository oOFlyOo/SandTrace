Sleepless Footprints - Version 1.2.0
==============================================

Scripts will work perfectly on Mobile / PC / Consoles and while this scene was created with Unity 5.5.0f3, it will work on all versions of Unity5 by setting it up in the same way. 
 
Tutorial Video on how it all works and how to add Sleepless Footprints to any other asset you may have or get from the Asset Store and customize it to work for any animal or humanoid character.
https://youtu.be/KTAN-tl2tWE

Technical Details you may be interested in 
 
1. In the demo scene, we are using a copy of a Standard Asset for a 3rd Person Character Controller. 
2. Added a Zoom script - And updated the input settings to allow a Right Click of Mouse to trigger the zoom in and out. 
3. We modified the ThirdPersonController and the AIControllers to have the LegPlatform GameObjects and relevant attached scripts placed correctly in the character hierarchy. An example of this can be found by navigating through the ThirdPersonController, like so. ThirdPersonController->EthanSkeleton->EthanHips->EthanSpine->EthanLeftUpLeg->EthanLeftLeg->EthanLeftFoot->EthanLeftToe1>LeftLegPlatform 
4. Essentially, Trigger collisions are detected between the bottom of the foot, and the terrain. When this occurs, we simply place down a plane, aligned correctly, with the normal footprint image and play a footprint sound.  

We have added a snow scene as well as a dust scene to show the footprint working on different terrains...

The footprints are now also using a Pool manager for performance management

If you have any questions, comments or improvement suggestions, you can  email me: daceian@yahoo.com.au
Other contact details available on our website: www.sleeplessentertainment.com.au