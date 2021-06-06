# TaleSpire-BVH_TS_Converter
This is an application for converting animations in BVH files to JSON files compatible with Lord Ashes' General Animation Plugin for TS.
The General Animation plugin allows applying animations to characters without the character asset having to have the animation in its assetBundle.
This allows animations to be reused on may minis without requiring the mini assetBundle to include those animations.

For example, Mixamo animations can be loaded into Blender, saved as BVH and then converted for use with the General Animation Plugin.
The Mixamo database of animations is vast and available for free (with a free account). However, the same process should work on animations
from different sources and/or exported using a different 3D editing software.

# Usage

Place the desired BVH file into the same directory as the application. Rename the file as desired. Then run the converter in one of two possible ways:

```Command Prompt
BVH_TS_Converter *Sample.bvh*
```

or

```Command Prompt
BVH_TS_Converter *Sample.bvh* *SkeletonName*
```

Where *Sample.bvh* is the file name of the file to be converted. This parameter does support a path if the file is not copied to the same folder.
Where *Skeleton* is the name of a skeleton whose bones are defined is a corresponding *Skeleton.txt* file (e.g. Minimal.txt). See below.

The converter will run and produce a file with the same path and filename as the input but with the extension changed from BVH to JSON.

# Skeletons

The converter allows the specification of many different skeletons. These can be both human or non-human. Configurations for a few different
human skeletons are included (Full, Minimal and Hands). By default, the Blender BVH exporters the entire skeleton. If the entire skeleton is
used for the animation then the animation will likely animate correctly but since each bone is being animated, the animation process will be
very CPU intensive especially if more than one animation is being processed.

As such, if not all bones are necessary for the animation, a reduce skeleton can be used which may not be able to animate some bones but will
significantly reduce the CPU usage needed to animate the animation. For example, the Hands skeleton keeps all the bones of the full skeleton
but removes all finger bones. This is a significant reduction since normally each finger has multiple bones. The Minimal skeleton takes this
even further and removes hands from the bone list meaning arms can be animated but not the hands. Typically for minis the Minimal or Hands
skeletons are sufficient as opposed to using the Full skeleton.

# Custon And Non-Human Skeletons

Both the General Animation Plugin and this converter are not limited to a specific skeleton structure. Sample configruations for a Mixamo/Unity
skeleton structure are provided but the user can easy add skeleton structures of his/her own by creating a text file and entering the names
of the bones (one per line). When the General Animation Plugin tries to apply a animation to a character, it tries to look up each of the
specified bones in the mini's bone structure and animates the ones that match.

Obviously if animations are to be shared between many minis then those minis needs to have a compatible skeleton structure (i.e. a skeleton
structure that uses the same bone names) but this approach means you can make animations for non-human skeletons or unique skeletons. Such
animations may not be re-usable between multiple mini but can still be used on mini of the appropriate skeleton type.

This leads to some interesting possibilities...

## Extended Skeleton

Since animations that specify bones which the character does not have are ignored. Animations can have animation for extra bones, such as
flapping wings, which would be ignored on characters who don't have them (at the expense of some CPU) but would be animated on characters
that do.

## Non-Human Skeletons

The skeleton file for the converter can list totally non-human bones for use with non-human skeleton structures. Obviously these types of
animations will be useless when applied to human minis but the animations can still be used on mini of the appropriate skeleton structure. 


