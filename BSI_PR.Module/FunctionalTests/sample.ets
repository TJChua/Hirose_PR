;#DropDB BSI_PREasyTest

#Application BSI_PR.Win
#Application BSI_PR.Web

*FillForm
 User Name = Admin
*Action Log In

#IfDef BSI_PR.Win

*Action About...
 
*Action OK

#EndIf

#IfDef BSI_PR.Web
*Action Navigation(My Details)
#EndIf
