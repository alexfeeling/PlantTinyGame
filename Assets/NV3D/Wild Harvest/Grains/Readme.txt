// Wild Harvest: Grains // 2022 //

- Package setup for built-in render pipeline by default. 

- If using URP or HDRP make sure to swap to the correct shaders by importing the corrosponding package in the shader folder.

Note: For HDRP projects some setup on the material will be required, such as tweaking colour values, setting double sided and alpha clip to an appropriate level. 
There is also a basic foliage diffusion profile included which can be used to give translucency to the plants, this can be assigned on the individual materials.

- Crop growth and spawn controlled by script graphs.  

- If scripts are not showing on the prefabs then check Unity's Visual Scripting package is installed. 

- In demo level simply play scene and click on a bush to trigger the growth stages. You can also click the soil to toggle between dry and wet.

- Soil uses the red channel on the vertex colours to define wetness area when toggled on.

- NV3D/Wild Harvest/Grains/Prefabs/ParticleMeshPrefabs folder and contents 

Foliage Masks Breakdown:

Mask texture
R = ID mask for Stem (~0.25), flower (~0.5) and fruit (~0.75) tints.
G = Translucency (simple boost to emission)
B = AO 
A = Smoothness

Vertex Color
R = AO
G = Leaf variation (Height based with some noise)
B = Empty
A = Empty

Shaders made with Amplify Shader Editor. 

Thank you for purchasing Wild Harvest: Grains, please consider leaving a review and checking out my other packs!

https://assetstore.unity.com/publishers/314

// For questions and queries please contact me here: matt.nv3d@gmail.com //