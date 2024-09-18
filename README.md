![Swatchr Logo](https://i.imgur.com/zdcXd53.png "Swatchr Logo")

>by [@00jknight](http://www.00jknight.com/)

# Overview
Swatchr allows Unity developers to store color palettes inside scriptable objects in their projects. Renderers, particle systems, cameras and shaders can then reference colors as keys into swatches. Swatches can be replaced and updated at runtime, and the changes will propagate. Swatches can be exported to Unity's built in color picker system. Swatchr can be easily extended to custom components by implementing the SwatchrColorApplier interface.

# Features
* Color palettes stored as scriptable objects in your project and repository
* Import swatches from .ase & .png files 
* Import swatches from MagicaVoxel's palette export (See Note)
* Export swatches to Unity's built in Color Picker tool
* Components to apply color to Mesh Renderer, Sprite Renderer, Particle System, Light and Camera clear color
* Nice editor UI
* Comes with the AAP-Splendor-128 color palette designed by [@AdigunPolack](https://twitter.com/adigunpolack/status/993524761019015168)

## Creating a Swatch
Create an empty Swatch by going to SOSXR -> Create New Swatch. Click on the .asset file to view it's Editor UI in the Inspector. Add colors to it by clicking the + button. Click on the color next to the selected color to use Unity's color picker to pick a color.

By default each SwatchrColorApplier derived class will try to load a swatch called "Project Wide Swatch". This will be the default swatch for all components. 

Create the color-palette specific swatches, and name them accordingly.Then, use the Replace button to swap out the colors in the Project Wide Swatch for the ones to your liking. This will update all the references to that swatch in the project. You may (sometimes) need to hit Play to see the changes.

## Using Swatches
### Renderer
Add a SwatchrRenderer component to a GameObject with a MeshRenderer or SpriteRenderer. Now you can select a color from the Swatch by selecting the key in the SwatchrColor field. 
### Light
Add a SwatchrLight component to a GameObject with a Light component. Now you can select a color from the Swatch by selecting the key in the SwatchrColor field.
### Ambient Light
Add a SwatchrAmbientLightColor component to a GameObject. Now you can select a color from the Swatch by selecting the key in the SwatchrColor field. It will change the ambient light color of the scene. You can see this in the Lighting window -> Ambient Color.

## URP Shaders
Use the URP versions for Universal Render Pipeline projects. Use the other versions for Built-In Render Pipeline projects.

## Swapping Swatches
Create a swatch for your project, eg) "Project Wide Swatch", and then use that swatch everywhere. Now duplicate that swatch and create alternative swatches with the same number of col. Now use the Replace button on MySwatch to swap color palettes.

## Finding Color Palettes
For instance here at [Color Hunt](https://colorhunt.co/).


# Technical Details
* Swatch.cs is a scriptable object that contains an array of colors.
* SwatchrColor.cs exposes a color property that uses an integer key and a Swatch to return a color.
* SwatchrColorApplier.cs is an interface for a component that will automatically apply a SwatchrColor to a GameObject when the Swatch changes.
* SwatchrRenderer.cs is one of several implementations of SwatchrColorApplier. This class applies the SwatchrColor to a MeshRenderer or SpriteRenderer.
* Other examples of SwatchrColorApplier are SwatchrLight, SwatchrParticleSystem & SwatchrAmbientLightColor.

# Screenshots
## Swapping Swatches
![Swatch Asset](https://i.imgur.com/81pPvFg.gif "Swatch Gif")
## Swatch Asset
![Swatch Asset](https://i.imgur.com/Trtywop.png "Swatch Asset")
## Using a Swatch in the Color Picker
![Color Picker](https://i.imgur.com/qCEx68a.png "Color Picker")