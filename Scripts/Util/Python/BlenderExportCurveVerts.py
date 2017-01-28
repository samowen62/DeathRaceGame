'''
Steps to export points of Bezier Spline:
	1) Make copy of spline object
	2) Alt + C -> convert curve to mesh
	3) Select new mesh in object mode
	4) Run this script in the text editor 
		to print out the xml data
'''

import bpy

def ftos(fl):
    return "{:.4f}".format(fl)

def createPointsXML(curve):
    xmlString = ""
    
    xmlString += "<BezierPoints>\n"
    for vert in curve.vertices:
        xmlString += "<Point>\n"
        xmlString += "<x>" + ftos(vert.co[0]) + "</x>\n"
        xmlString += "<y>" + ftos(vert.co[1]) + "</y>\n"
        xmlString += "<z>" + ftos(vert.co[2]) + "</z>\n"
        xmlString += "</Point>\n"

    xmlString += "</BezierPoints>"
	
	
    #note: vert.normal is normal of the mesh not the track so we can't use it    

    print(xmlString)

def createWidthXML(curve)
	xmlString = ""
    
    xmlString += "<Widths>\n"			
	for vert in curve.splines[0].bezier_points:
		xmlString += "<width>" + ftos(vert.radius) + "</width>\n"
		
	xmlString += "</BezierPoints>"

	print(xmlString)
	
createBezierXML(bpy.context.active_object.data)


