import speech_recognition as sr
import time
import os
import playsound
import pyttsx3
import pyaudio
import wave
import wikipedia
import subprocess
import webbrowser
import pyodbc
from gtts import gTTS
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
    if requestCommnad.find("tìm kiếm") != -1:
        request = requestCommnad.replace("tìm kiếm","")
        query = getVariable("*" , "search_command" , "request like '%" +request+ "%' ").fetchone()
        requestCommnad = request
    else:
        query = getVariable("*" , "search_command" , "request like '%" +requestCommnad+ "%' ").fetchone()

    #do command
    wikipedia.set_lang("vi")

    if(query is None ):
        print("Trợ lý : " + wikipedia.summary( requestCommnad ) )

        responeValues = wikipedia.summary(requestCommnad)

        query="N'{0}',N'{1}'".format(requestCommnad,responeValues)

        deleteVariable("search_command",requestCommnad)

        insertVariable("search_command",query)
    else:
        print("Trợ lý : " + str(query[2]))
#Open something
def open_file(requestCommnad):
    # 1 : open file / folder  ; 2 : go to link

    command = requestCommnad.replace("mở ","")
    
    query = getVariable("*" , "open_command" , "command= N'" +command+ "' ").fetchone() 

    #do command
    if(query is not None):
        try:
            if(str(query[3]) == "2"):
                webbrowser.open( str(query[2]))
            elif(str(query[3]=="1")):
                try:
                    subprocess.Popen([str(query[2])])
                except:
                    os.startfile(str(query[2]))
                
        except:
            assistant_say("Tôi không hiểu lệnh đó")
        # if((requestCommnad.find("internet") != -1 or requestCommnad.find("google") != -1)):
        #     subprocess.Popen(['C:\Program Files (x86)\Google\Chrome\Application\chrome.exe'])
                
        # if(requestCommnad.find("facebook") != -1):
        #     webbrowser.open("http://www.facebook.com/")
    else:
        assistant_say("Tôi không hiểu lệnh đó")

"""Database"""
def connectDB():
    return pyodbc.connect('Driver={SQL Server};' 'Server=SHJN\SQLEXPRESS;' 'Database=Assistant;' 'Trusted_Connection=yes;') #connection string

#Select
def getVariable(query , table , condition):
    cursor  = connectDB().cursor()

    query = "select "+query+" from "+table+" where "+condition+" "

    return cursor.execute(query)

#Insert
def insertVariable(table , values):
    values = values.replace( "","" )
    connect=connectDB()
    cursor  = connect.cursor()
    
    query = 'insert into {0} values({1})'.format(table,values)

    cursor.execute(query)
    connect.commit()

#Delete
def deleteVariable(table,condition):
    connect=connectDB()
    cursor  = connect.cursor()

    query = "DELETE FROM "+table+" WHERE command =N'"+condition+"' "

    cursor.execute(query)
    connect.commit()

"""Speech"""
def assistant_say(temp):
    tts = gTTS(text = temp , lang = "vi" , slow=False)
    tts.save("Test.mp3")
    playsound.playsound("Test.mp3",True)
    engine.runAndWait()
    os.remove("Test.mp3")
"""Open"""
ass_listen = sr.Recognizer()
ass_say= " Tôi có thể giúp gì bạn ?"
assistant_say(ass_say)

with sr.Microphone() as mic :
    # engine.say(ass_say )
    # print( "Assistant : " + ass_say)
    # engine.runAndWait()
    # engine.stop()

    #config mic in
    mic.dynamic_energy_threshold = False    #config
    audio = ass_listen.listen(mic,phrase_time_limit=3)
    
    #do request
    try:
        you = ass_listen.recognize_google(audio,language="vi-VN").lower()
        print("Bạn : "+you)
        #Scale request and do command
        if(you.find("mở") != -1 ):
            open_file(you)
        elif(you.find("tìm kiếm") != -1 ) :
            search(you)
        else:
            search(you)
    except:
        assistant_say("Tôi không thể nghe thấy")




