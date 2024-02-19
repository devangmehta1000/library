# library
A library system built as a part of coding challenge, offering a basic search functionality.

There are two endpoints - GenerateToken and Search.

#GenerateToken
GenerateToken endpoint takes a role parameter and creates a jwt using symmetric key. The different roles is used to test authorization.

#Search
The token generated above is then used (in the Authorization header) to authenticate and authorise the search requests.
The search endpoint takes a free text input and searches against - title, genre, type, publication year, author name.
The partial matches are performed using fuzzy search. This takes care of common typos - e.g. NonFictional will get match to Non-Fictional.

A SqlLite database is used for this challenge. The sample data is as shown below

#Books
![image](https://github.com/devangmehta1000/library/assets/29085371/09b048c4-9039-4f03-a6cc-e1b3503b6235)

#Author
![image](https://github.com/devangmehta1000/library/assets/29085371/65375a6a-9ebc-4b36-9173-ae512755f8e2)
