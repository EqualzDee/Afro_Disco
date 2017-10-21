/*
Author: Matthew J. Collins (Délé)
Date: 12.21.09
http://www.DeleVinciStudios.com
 */
 
// align in the x translation axis
@MenuItem ("LineMup/Align/Translation X")
static function AlignmentTransX () {
    // execute alignment for the x axis
    AlignOrDistribute(false, "transX");
}
 
// determine if the function can be executed.
@MenuItem ("LineMup/Align/Translation X", true)
static function ValidateAlignmentTransX () {
    // only return true if there is a transform in the selection.
    return (Selection.activeTransform != null);
}
 
// align in the y translation axis
@MenuItem ("LineMup/Align/Translation Y")
static function AlignmentTransY () {
    // execute alignment for the y axis
    AlignOrDistribute(false, "transY");
}
 
// determine if the function can be executed.
@MenuItem ("LineMup/Align/Translation Y", true)
static function ValidateAlignmentTransY () {
    // only return true if there is a transform in the selection.
    return (Selection.activeTransform != null);
}
 
// align in the z translation axis
@MenuItem ("LineMup/Align/Translation Z")
static function AlignmentTransZ () {
    // execute alignment for the z axis
    AlignOrDistribute(false, "transZ");
}
 
// determine if the function can be executed.
@MenuItem ("LineMup/Align/Translation Z", true)
static function ValidateAlignmentTransZ () {
    // only return true if there is a transform in the selection.
    return (Selection.activeTransform != null);
}
 
// align the rotation
@MenuItem ("LineMup/Align/Rotation")
static function AlignmentRotation () {
    // execute alignment in all axes
    AlignOrDistribute(false, "rotAll");
}
 
// determine if the function can be executed.
@MenuItem ("LineMup/Align/Rotation", true)
static function ValidateAlignmentRotation () {
    // only return true if there is a transform in the selection.
    return (Selection.activeTransform != null);
}
 
// align in the x scale axis
@MenuItem ("LineMup/Align/Scale X")
static function AlignmentScaleX () {
    // execute alignment for the x axis
    AlignOrDistribute(false, "scaleX");
}
 
// determine if the function can be executed.
@MenuItem ("LineMup/Align/Scale X", true)
static function ValidateAlignmentScaleX () {
    // only return true if there is a transform in the selection.
    return (Selection.activeTransform != null);
}
 
// align in the y scale axis
@MenuItem ("LineMup/Align/Scale Y")
static function AlignmentScaleY () {
    // execute alignment for the y axis
    AlignOrDistribute(false, "scaleY");
}
 
// determine if the function can be executed.
@MenuItem ("LineMup/Align/Scale Y", true)
static function ValidateAlignmentScaleY () {
    // only return true if there is a transform in the selection.
    return (Selection.activeTransform != null);
}
 
// align in the z scale axis
@MenuItem ("LineMup/Align/Scale Z")
static function AlignmentScaleZ () {
    // execute alignment for the z axis
    AlignOrDistribute(false, "scaleZ");
}
 
// determine if the function can be executed.
@MenuItem ("LineMup/Align/Scale Z", true)
static function ValidateAlignmentScaleZ () {
    // only return true if there is a transform in the selection.
    return (Selection.activeTransform != null);
}
 
// distribute in the x translation axis
@MenuItem ("LineMup/Distribute/Translation X")
static function DistributeTransX () {
    // execute distribution for the x axis
    AlignOrDistribute(true, "transX");
}
 
// determine if the function can be executed.
@MenuItem ("LineMup/Distribute/Translation X", true)
static function ValidateDistributeTransX () {
    // only return true if there is a transform in the selection.
    return (Selection.activeTransform != null);
}
 
// distribute in the y translation axis
@MenuItem ("LineMup/Distribute/Translation Y")
static function DistributeTransY () {
    // execute distribution for the y axis
    AlignOrDistribute(true, "transY");
}
 
// determine if the function can be executed.
@MenuItem ("LineMup/Distribute/Translation Y", true)
static function ValidateDistributeTransY () {
    // only return true if there is a transform in the selection.
    return (Selection.activeTransform != null);
}
 
// distribute in the z translation axis
@MenuItem ("LineMup/Distribute/Translation Z")
static function DistributeTransZ () {
    // execute distribution for the z axis
    AlignOrDistribute(true, "transZ");
}
 
// determine if the function can be executed.
@MenuItem ("LineMup/Distribute/Translation Z", true)
static function ValidateDistributeTransZ () {
    // only return true if there is a transform in the selection.
    return (Selection.activeTransform != null);
}
 
// distribute the rotation
@MenuItem ("LineMup/Distribute/Rotation")
static function DistributeRotation () {
    // execute distribution in all axes
    AlignOrDistribute(true, "rotAll");
}
 
// determine if the function can be executed.
@MenuItem ("LineMup/Distribute/Rotation", true)
static function ValidateDistributeRotation () {
    // only return true if there is a transform in the selection.
    return (Selection.activeTransform != null);
}
 
// distribute in the x scale axis
@MenuItem ("LineMup/Distribute/Scale X")
static function DistributeScaleX () {
    // execute distribution for the x axis
    AlignOrDistribute(true, "scaleX");
}
 
// determine if the function can be executed.
@MenuItem ("LineMup/Distribute/Scale X", true)
static function ValidateDistributeScaleX () {
    // only return true if there is a transform in the selection.
    return (Selection.activeTransform != null);
}
 
// distribute in the y scale axis
@MenuItem ("LineMup/Distribute/Scale Y")
static function DistributeScaleY () {
    // execute distribution for the y axis
    AlignOrDistribute(true, "scaleY");
}
 
// determine if the function can be executed.
@MenuItem ("LineMup/Distribute/Scale Y", true)
static function ValidateDistributeScaleY () {
    // only return true if there is a transform in the selection.
    return (Selection.activeTransform != null);
}
 
// distribute in the z scale axis
@MenuItem ("LineMup/Distribute/Scale Z")
static function DistributeScaleZ () {
    // execute distribution for the z axis
    AlignOrDistribute(true, "scaleZ");
}
 
// determine if the function can be executed.
@MenuItem ("LineMup/Distribute/Scale Z", true)
static function ValidateDistributeScaleZ () {
    // only return true if there is a transform in the selection.
    return (Selection.activeTransform != null);
}
 
static function AlignOrDistribute(shouldDist, theAxis){
 
    // create some variables to store values
    var firstObj = Selection.activeTransform;
    var furthestObj = firstObj;
    var firstVal = 0.0;
    var furthestVal = 0.0;
    var curDist = 0.0;
    var lastDist = 0.0;
    var selCount = 0;
 
    // collect the number of tranforms in the selection and find the object that is furthest away from the active selected object
    for (var transform in Selection.transforms)
    {
        // collect the current distance
        curDist = Vector3.Distance(firstObj.position, transform.position);
 
        // get the object with the greatest distance from the first selected object
        if (curDist > lastDist)
        {
            furthestObj = transform;
            lastDist = curDist;
        }
 
        // increment count
        selCount += 1;
    }
 
    // distribute or align?
    if (shouldDist)
    {
        // collect the first value and furthest value to distribute between
        switch (theAxis)
        {
            case "transX":
                firstVal = firstObj.position.x;
                furthestVal = furthestObj.position.x;
                break;
            case "transY":
                firstVal = firstObj.position.y;
                furthestVal = furthestObj.position.y;
                break;
            case "transZ":
                firstVal = firstObj.position.z;
                furthestVal = furthestObj.position.z;
                break;
            case "scaleX":
                firstVal = firstObj.localScale.x;
                furthestVal = furthestObj.localScale.x;
                break;
            case "scaleY":
                firstVal = firstObj.localScale.y;
                furthestVal = furthestObj.localScale.y;
                break;
            case "scaleZ":
                firstVal = firstObj.localScale.z;
                furthestVal = furthestObj.localScale.z;
                break;
            default:
                break;
        }	
 
        // calculate the spacing for the distribution
        var objSpacing = (firstVal - furthestVal) / (selCount - 1);
        var curSpacing = objSpacing;
        var rotSpacing = 1.0 / (selCount - 1);
        var curRotSpacing = rotSpacing;
 
        // update every object in the selection to distribute evenly
        for (var transform in Selection.transforms)
        {
            switch (theAxis)
            {
                case "transX":
                    if ((transform != firstObj) && (transform != furthestObj))
                    {
                        transform.position.x = firstVal - curSpacing;
                        curSpacing += objSpacing;
                    }
                    break;
                case "transY":
                    if ((transform != firstObj) && (transform != furthestObj))
                    {					
                        transform.position.y = firstVal - curSpacing;
                        curSpacing += objSpacing;
                    }
                    break;
                case "transZ":
                    if ((transform != firstObj) && (transform != furthestObj))
                    {
                        transform.position.z = firstVal - curSpacing;
                        curSpacing += objSpacing;
                    }
                    break;
                case "rotAll":
                    if ((transform != firstObj) && (transform != furthestObj))
                    {
                        transform.rotation = Quaternion.Slerp (firstObj.rotation, furthestObj.rotation, curRotSpacing);
                        curRotSpacing += rotSpacing;
                    }
                    break;
                case "scaleX":
                    if ((transform != firstObj) && (transform != furthestObj))
                    {
                        transform.localScale.x = firstVal - curSpacing;
                        curSpacing += objSpacing;
                    }
                    break;
                case "scaleY":
                    if ((transform != firstObj) && (transform != furthestObj))
                    {
                        transform.localScale.y = firstVal - curSpacing;
                        curSpacing += objSpacing;
                    }
                    break;
                case "scaleZ":
                    if ((transform != firstObj) && (transform != furthestObj))
                    {
                        transform.localScale.z = firstVal - curSpacing;
                        curSpacing += objSpacing;
                    }
                    break;
                default:
                    break;
            }
        }
    }
    else
    {	
        // snap every object in the selection to the first objects value
        for (var transform in Selection.transforms)
        {
            switch (theAxis)
            {
                case "transX":
                    transform.position.x = firstObj.position.x;
                    break;
                case "transY":
                    transform.position.y = firstObj.position.y;
                    break;
                case "transZ":
                    transform.position.z = firstObj.position.z;
                    break;
                case "rotAll":
                    transform.rotation = firstObj.rotation;
                    break;
                case "scaleX":
                    transform.localScale.x = firstObj.localScale.x;
                    break;
                case "scaleY":
                    transform.localScale.y = firstObj.localScale.y;
                    break;
                case "scaleZ":
                    transform.localScale.z = firstObj.localScale.z;
                    break;
                default:
                    break;
            }
        }
    }
}