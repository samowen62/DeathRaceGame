'''
OBSOLETE: NOT USING BEZIER CURVES

This is a script used to print a bezier curve's points in xml format
to be read in by the game. To use the bezier curve must be selected in
object mode and this script can be ran.
'''

import bpy

def ftos(fl):
    return "{:.4f}".format(fl)

def createBezierXML(curve):
    xmlString = ""
    
    for spline in curve.splines:
        xmlString += "<BezierPoints>\n"
        
        for kp in spline.bezier_points:
            #print("%r"%[kp.handle_left, kp.co, kp.handle_right])

            xmlString += "<Point>\n"
            xmlString += "<x>" + ftos(kp.handle_left[0]) + "</x>\n"
            xmlString += "<y>" + ftos(kp.handle_left[1]) + "</y>\n"
            xmlString += "<z>" + ftos(kp.handle_left[2]) + "</z>\n"
            xmlString += "</Point>\n"
            xmlString += "<Point>\n"
            xmlString += "<x>" + ftos(kp.co[0]) + "</x>\n"
            xmlString += "<y>" + ftos(kp.co[1]) + "</y>\n"
            xmlString += "<z>" + ftos(kp.co[2]) + "</z>\n"
            xmlString += "</Point>\n"
            xmlString += "<Point>\n"
            xmlString += "<x>" + ftos(kp.handle_right[0]) + "</x>\n"
            xmlString += "<y>" + ftos(kp.handle_right[1]) + "</y>\n"
            xmlString += "<z>" + ftos(kp.handle_right[2]) + "</z>\n"
            xmlString += "</Point>\n"
            
        xmlString += "</BezierPoints>"
            

    print(xmlString)

createBezierXML(bpy.context.active_object.data)