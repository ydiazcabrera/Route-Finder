# Route-Finder

Live website: bit.ly/primepath  

The purpose of this project is to provide transparency to the citizens of Grand Rapids about air quality at at a hyper-local and real-time scale. Specifically, this application provides users with a walking or biking path through the city that avoids areas that currently have poor air quality. The user enters starting and ending locations on a map and they are provided with their path. 

Air quality data is pulled in from 12 Seamless IoT sensors placed throughout the city. You can read more about the air quality initiative here: https://www.rapidgrowthmedia.com/features/09282017_OST_Air_Quality.aspx . 

This is currently a proof of concept. There are not enough sensors throughout the city to make this a meaningful application for the majority of the city's population. We did not consult with any experts while creating this app, so there are likely some areas that need improvement. Most notablbly, our formulas and bounding boxes around sensors with poor air quality need to be reviewed before making this app available to the public. 

We hope to continue to develop this app by incorporating sensors as they are added and pulling data from AWS in real time. 
