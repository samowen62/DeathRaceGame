'''
Steps to export points of Bezier Spline:
	1) Make copy of spline object
	2) Alt + C -> convert curve to mesh
	3) Select new mesh in object mode
	4) Run this script in the text editor 
		to print out the xml data
'''

#did scale y by .5 .5 .75 1.1 = .206 is good :)
#camera gets closer to ship when off tracks
import bpy

def ftos(fl):
    return "{:.4f}".format(fl)

def createPointsXML(curve):
    xmlString = ""
    
    xmlString += "<TrackPoints>\n"
    for vert in curve.vertices:
        xmlString += "<Point>\n"
        xmlString += "<x>" + ftos(vert.co[0]) + "</x>\n"
        xmlString += "<y>" + ftos(vert.co[1]) + "</y>\n"
        xmlString += "<z>" + ftos(vert.co[2]) + "</z>\n"
        xmlString += "</Point>\n"

    xmlString += "</TrackPoints>"
	
	
    #note: vert.normal is normal of the mesh not the track so we can't use it    

    print(xmlString)

def createWidthXML(curve):
    xmlString = ""
    xmlString += "<BezierPoints>\n"
    
    for vert in curve.splines[0].bezier_points:
        xmlString += "<BezierPoint>\n"
        xmlString += "<x>" + ftos(vert.co[0]) + "</x>\n"
        xmlString += "<y>" + ftos(vert.co[1]) + "</y>\n"
        xmlString += "<z>" + ftos(vert.co[2]) + "</z>\n"
        xmlString += "<width>" + ftos(vert.radius) + "</width>\n"
        xmlString += "</BezierPoint>\n"
    xmlString += "</BezierPoints>"

    print(xmlString)

createPointsXML(bpy.context.active_object.data)	
createWidthXML(bpy.context.active_object.data)


