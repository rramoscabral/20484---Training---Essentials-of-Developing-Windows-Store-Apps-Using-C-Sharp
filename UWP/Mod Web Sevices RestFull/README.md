# Módulo Web Services com Universal Windows Platform (UWP)

Um Web Service é um conjunto de métodos que são invocados por outros programas independente da linguagem de programação utilizada, com o objetivo de transferência de dados.

Há dois protocolos de comunicações utilizados:
 * Simple Object Access Protocol (SOAP), que utiliza XML para enviar e receber mensagens.
 * Representational State Transfer (REST), que utiliza JSON para enviar e receber mensagens.


### Demostração
A demostração é composta com dois projetos e ambos utilizam REST.

 * **ASPDotNETCoreRESTFulWS**
   * Baseado no StudentRegistrationDemo3 do Abhijit Pritam Dutta (https://github.com/prateekparallel/StudentRegistrationDemo3)
   * Framework: Dot Net Core 2.1
   * IIS Express 
   * App URL: http://localhost:13758

</br>

 * **UWPRestFullClient**
   * References:
      * Microsoft.NETCore.UniversalWindowsPlatform
	  * Newtonsoft.JsonResult
	  * Universal Windows
	  * Windows Desktop Extensions for the UWP
   
   
</br>
</br>

#### **ASPDotNETCoreRESTFulWS**

**Exemplo de um documento Json**

```json
 {
  "ID":"5",
  "Name":" Teste 001",
  "Email":"001@test.test"
}
```

**Métodos do Web Service**

Obter uma lista de todas as pessoas
 * Método HTTP: GET
 * Recurso: /api/pearsonretrive
 * Media Type: application/json

</br>

Registar uma pessoa
 * Método HTTP:  POST
 * Recurso: /api/pearsonregistration/InsertPearson
 * Media Type: application/json

</br>

Atualizar o registo de uma pessoa 
 * Método HTTP:   PUT
 * Recurso:  /api/pearsonupdate
 * Media Type: application/json

</br>

Remover um registo de uma pessoa
 * Método HTTP: DELETE
 * Recurso:  /api/pearsondelete/pearson/remove/
 * Parametro do recurso: ID 
 * Media Type: application/json

</br>
</br>

#### **UWPRestFullClient**

Foram aplicados alguns componentes do curso. Por exemplo UI, Tasks, Binding e outros.

