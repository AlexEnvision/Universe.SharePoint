# Контроль версий    
Для удобства проект нужно клонировать по локальному пути C:\P\Universe.Framework.SharePoint\git (но это совсем не обязательно)
*  master - основная рабочая ветка, изменения в нее попадают только через MergeRequests
*  develop - вторая основная рабочая ветка, изменения в нее попадают также, через MergeRequests

Для каждой задачи разработки/исправления ошибки заводится отдельный бранч.  

*Создается прямо из задачи, необходимо в названии задачи, ошибки, перед созданием указывать в скобках на английском языке не очень длинное наименование бранча.*

## Правила формирования сообщений к комиту
Сообщение может быть многострочным, например:  
#10 [WebApp Auth]: Добавлена авторизация в web-приложении.  
#10 [Common Structure]: Изменены ссылки на проекты.  
#10 [Monitoring, Delta]: Удален артефакт.  

где:
*  #10 - номер задачи (как правило совпадает с номером бранча) - в gitlab будет превращаться в ссылку на задачу,
  а при наведении мыши покажет название задачи
*  "WebApp Auth:"" название функционала, в рамках которого делается коммит. Обязательно указывается с двоеточием на конце.
*  "Добавлена авторизация в web-приложении." - текст описывающий, что было сделано. Обязательно с точкой в конце, завершающей предложение.
*  Может быть перечислено несколько действий записываемые подобным образом
  н-р "#10 WebApp Auth: Подключен контейнер Unity в проекте WebApp. Подключен контейнер Unity в проекте Core.""

*  [\~] - указываем в начале строки коммита, если мержим файлы вручную (автоматом git сам формирует сообщение). Сообщение должно быть вида:  
    [\~] Merge from develop to 11-build-sp-microservices  
    или  
    [\~] Merge from develop to #11  

# Средства разработки
Для разработки использовать VS 2017, VS2019 дополнительно должны быть установлены:
*  поддержка PowerShell проектов (это устанавливается при устаноке VS)
*  git интегрируемый в студию (это устанавливается при устаноке VS)
*  Если после обновления из ветки develop, у проектов слетели References, а в ошибке фигурирует "NuGet", можно сделать следующее:
   на Solution нажать правую кнопку мыши и выбрать и выбрать Restore NuGet Packages. Затем Clean solution, build solution.
*  Чтобы NuGet ресторил сборки автоматически, нужно сделать следующее:
   в меню Tools - Options - NuGet Package Manager выставить 2 галки:
   - Allow NuGet to download missing packages;
   - Automaticall check for missing packages during build in Visual Studio.
*  Опционально экстэншн к студии: GruntLauncher. При нажатии на gruntfile.js правой кнопкой мыши, в подменю Gulp, показывает меню из возможных для выполнения задач по компоновки js/css и т.д.
*  SQL Server 2014 и выше
* Visual Stidio Code 1.42 и выше

# Версии ПО/платформ/ библиотек
*  SQL 2014 SP3 (Build 11.0.6607.3) 
*  SharePoint 2013 (Build 15.0.5023.1000), Nintex Workflow 2013 (3.1.1.0) - International

# Обратить внимание
*  На кодировку файлов (особенно *.ps1), должна быть UTF-8

# Coding Style
*  По оформлению кода придерживаться настроек решарпера в файле ReSharper.DotSettings
* !Перед коммитом обязательно выполнять реформат измененного кода по схеме ResharperSln (исключением является кодоген и код
 сгененированны T4 шаблоном) 
*  При реформате выбирать профиль ResharperSln для полного реформата, ResharperSln NoSort для классов в которых нельзя изменять порядок
*  Блок catch, если в блоке catch, нет `throw ...;`, то необходимо указать комментарий почему его тут нет, например
 как в примере ниже  
 Если в блоке catch создается новый инстанс ошибки, то обязательно необходимо указать исходную ошибку, или комментарий
 почему исходная ошибка не должна указываться.
```c#
catch (Exception ex) {
    _log.Unexpected(ex);
    //throw; Что бы здесь не произошло, это не должно повлиять на выполнение всего остального
}
````

# Примеры использования ...
## Формирование выборок по конкретным условиям

```c#
var accessMatrixMi = MetaInfo.UniverseWeb.AccessMatrixList;
var accessMatrixList = Web.GetSpListByUrl(accessMatrixMi.WebRelativeUrl);
var field = accessMatrixList.Fields.GetField(fieldInternalNameArgument) as SPFieldMultiChoice;
if (field == null)
{
    return false;
}

var items = accessMatrixList.GetItemsByQuery(
        where: CamlHelper.GetCamlWhere(
            CamlHelper.CamlChain(
                CamlHelper.LogicalOperators.AND,
                CamlHelper.GetEqLookup(accessMatrixMi.UvUser.InternalName, Web.CurrentUser.ID))),
        order: CamlHelper.GetCamlOrderBy(CamlHelper.GetCamlOrderByElement(accessMatrixMi.ID.InternalName, true)),
        viewFields: CamlHelper.BuildFieldsRef(
            field.InternalName))
    .Select(x => new
                {
                    Roles = x.GetMultiChoiceValue(field.InternalName)
                }).ToList();
````