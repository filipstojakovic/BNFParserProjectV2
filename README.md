# BNFParserProjectV2

creating regex using custom BNF with few additions, matching it with inputFile and creating XML of the given input. 
custom terminal nodes
broj_telefona - phone number regex = @"(((\()?(00|\+)387(65|66|51)(\))?)|(0(65|66|51)))?[- \/]?\d{3}[ -]?\d{3}";
mejl_adreasa - email regex = \w+([.]\w+){0,3}@\w+([-.]\w+){0,1}\.\w+([-.]\w+){0,3}
web_link - web link regex = ((https?:\/\/){0,1}(www\.){0,1}){1}\w+([-.]\w+){0,1}\.\w+([-.]\w+){0,3}([\w\/?=&]+)?
brojevna_konstanta - any real number regex = -?\d+(\.\d+)?
veliki_grad - using crawler takes top 200 cities in Europe

BNF example:
<student> ::= <ime> <prezime> <broj> <email> <grad> | <prezime>
<ime> ::= regex(\w+) | "Fipa" | "Filip"
<prezime> ::= "Stojakovic" | "Stoja"
<broj> ::= broj_telefona
<email> ::= mejl_adresa
<grad> ::= "iz" veliki_grad

Input Example:
Filip Stojakovic 065/123-321 filips@europe.com iz London

XML output:

<?xml version="1.0" encoding="utf-8"?>
<student>
	<ime>Filip</ime>
	<prezime>Stojakovic</prezime>
	<broj>065/123-321</broj>
	<email>filips@europe.com</email>
	<grad>iz London</grad>
</student>
