import speech_recognition as sr
import time
import os
import pyttsx3
import pyaudio
import wave
import wikipedia
import subprocess
import webbrowser
import pyodbc
"""Started"""


engine = pyttsx3.init()

"""VOLUME"""
volume = engine.getProperty('volume')   #getting to know current volume level (min=0 and max=1)
#print (volume)                          #printing current volume level
engine.setProperty('volume',1)    # setting up volume level  between 0 and 1

"""VOICE"""
voices = engine.getProperty('voices')       #getting details of current voice
#engine.setProperty('voice', voices[0].id)  #changing index, changes voices. o for male
engine.setProperty('voice', voices[1].id)   #changing index, changes voices. 1 for female

"""Command"""
#Search from wiki
def search(requestCommnad):
    ass_say= (wikipedia.summary(requestCommnad))
    engine.say(" This one ?")
    print("Assistant : " + ass_say)
    engine.runAndWait()      
    engine.stop()
  
#Open something
    # 1 : open file / folder  ; 2 : go to link
def open_file(requestCommnad):
    request = requestCommnad.split(" ")
    query = getVariable("*" , "open_command" , "request = '" +request[1]+ "' ").fetchone() 

    #do command
    if(str(query[3]) == "2"):
        webbrowser.open( str(query[2]))
    elif(str(query[2]==1)):
        subprocess.Popen([str(query[2])])


    # if((requestCommnad.find("internet") != -1 or requestCommnad.find("google") != -1)):
    #     subprocess.Popen(['C:\Program Files (x86)\Google\Chrome\Application\chrome.exe'])
            
    # if(requestCommnad.find("facebook") != -1):
    #     webbrowser.open("http://www.facebook.com/")
        


    else:
        engine.say(".I don't understand that command ?")
        print("Assistant : " + "I don't understand that command")
        engine.runAndWait()      
        engine.stop()

"""Database"""
def connectDB():
    return pyodbc.connect('Driver={SQL Server};' 'Server=SHJN\SQLEXPRESS;' 'Database=Assistant;' 'Trusted_Connection=yes;') #connection string

#Get
def getVariable(query , table , condition):
    cursor  = connectDB().cursor()

    query = "select "+query+" from "+table+" where "+condition+" "

    return cursor.execute(query)

#Edit
def editVariable(query , table , condition):
    cursor  = connectDB().cursor()

    cursor.execute("select "+query+" from "+table+" ")

    for row in cursor:
        print(row)

#Delete
def deleteVariable(table , condition):
    cursor  = connectDB().cursor()

    cursor.execute("delete from "+table+" where "+condition+"")

    for row in cursor:
        print(row)
"""Open"""
ass_listen = sr.Recognizer()
ass_say= " How can i help you"
with sr.Microphone() as mic :
    engine.say(ass_say )
    print( "Assistant : " + ass_say)
    engine.runAndWait()
    engine.stop()

    #config mic in
    mic.dynamic_energy_threshold = False    #config
    audio = ass_listen.listen(mic,phrase_time_limit=1)
    
    #do request
    try:
        you = ass_listen.recognize_google(audio).lower()
        print("You : "+you)
        #Scale request and do command
        if(you.find("open") != -1 ):
            open_file(you)
        elif(you.find("search") != -1 ) :
            search(you)
        else:
            engine.say(" I don't understand that command ?")
            print("Assistant : " + "I don't understand that command")
            engine.runAndWait()      
            engine.stop()
    except:
        engine.say(" I cant hear that ")
        print("Assistant : "+"I cant hear that ")
        engine.runAndWait()
        engine.stop()




