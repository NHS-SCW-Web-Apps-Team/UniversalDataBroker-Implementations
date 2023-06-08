import os
import string
import requests


def __getToken(client:string, secret: string, resource: string, tenant:string):
   r = requests.post("https://login.microsoftonline.com/{tenant}/oauth2/token?",
                            data= {"grant_type": "client_credentials", "client_id": client, "client_secret": secret, "resource": resource},
                           headers={"Content-Type": "application/x-www-form-urlencoded"})
   string = r.json()
  
   return string['access_token']

def __send(workflow: string, file: string, organisation: string, url: string, token: string):
    name, extension = os.path.splitext(file)
    print(extension)
    contentType = "application/octet-stream"
    if extension == ".csv":
        contentType = "text/csv"
    if extension == ".xls":
        contentType = "application/vnd.ms-excel"
    if extension == ".json":
        contentType = "application/json"
    if extension == ".zip":
        contentType = "application/zip"


    fileName = os.path.basename(file)

    f = open(file, 'rb')

    headers = {"Authorization": "Bearer " + token}
    myFiles = {'file': (fileName, open(file, 'rb'), 'application/octet-stream')}

    postUrl = f'{url}/api/files/upload/{organisation}/{workflow}'

    r = requests.post(postUrl, files=myFiles, headers=headers)

    #r = requests.post(f'{url}/api/files/upload/{organisation}/{workflow}', files={fileName: f}, headers={"Content-Type":contentType, "Authorization": "Bearer "+token})
    print(r.content)


def upload(workflow: string, file: string, organisation: string, url: string, client: string, secret: string, resource: string):
    if workflow == "":
        raise Exception("No workflow provided")
    if file == "":
        raise Exception("No filepath provided")
    if organisation == "":
        raise Exception("No organisation provided")
    if url == "":
        raise Exception("No url provided")
    if client == "":
        raise Exception("No client provided")
    if secret == "":
        raise Exception("No secret provided")
    if resource == "":
        raise Exception("No resource provided")
    token = __getToken(client, secret, resource)
    print(token)
    __send(workflow, file, organisation, url, token)


