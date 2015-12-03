Namespace Data
    ' The data model defined by this file serves as a representative example of a strongly-typed
    ' model.  The property names chosen coincide with data bindings in the standard item templates.
    '
    ' Applications may use this model as a starting point and build on it, or discard it entirely and
    ' replace it with something appropriate to their needs. If using this model, you might improve app 
    ' responsiveness by initiating the data loading task in the code behind for App.xaml when the app 
    ' is first launched.


    ''' <summary>
    ''' Generic item data model.
    ''' </summary>
    Public Class QuizDataItem
        Private Shared _baseUri As New Uri("ms-appx:///")

        Public Sub New(uniqueId As String, question_Number As String, question As String, answer1 As String, answer2 As String,
                       answer3 As String, correct As String)
            Me.UniqueId = uniqueId
            Me.Question_Number = question_Number
            Me.Question = question
            Me.Answer_1 = answer1
            Me.Answer_2 = answer2
            Me.Answer_3 = answer3
            Me.Correct = correct
        End Sub

        Private _uniqueId As String
        Public Property UniqueId As String
            Get
                Return _uniqueId
            End Get
            Private Set(value As String)
                _uniqueId = value
            End Set
        End Property


        Private _question_Number As String
        Public Property Question_Number As String
            Get
                Return _question_Number
            End Get
            Private Set(value As String)
                _question_Number = value
            End Set
        End Property


        Private _question As String
        Public Property Question As String
            Get
                Return _question
            End Get
            Private Set(value As String)
                _question = value
            End Set
        End Property

        Private _answer_1 As String
        Public Property Answer_1 As String
            Get
                Return _answer_1
            End Get
            Private Set(value As String)
                _answer_1 = value
            End Set
        End Property

        Private _answer_2 As String
        Public Property Answer_2 As String
            Get
                Return _answer_2
            End Get
            Private Set(value As String)
                _answer_2 = value
            End Set
        End Property

        Private _answer_3 As String
        Public Property Answer_3 As String
            Get
                Return _answer_3
            End Get
            Private Set(value As String)
                _answer_3 = value
            End Set
        End Property

        Private _correct As String
        Public Property Correct As String
            Get
                Return _correct
            End Get
            Private Set(value As String)
                _correct = value
            End Set
        End Property

        

        Public Overrides Function ToString() As String
            Return Me.Question
        End Function
    End Class

    ''' <summary>
    ''' Generic group data model.
    ''' </summary>
    Public Class QuizDataGroup
        Private Shared _baseUri As New Uri("ms-appx:///")

        Public Sub New(uniqueId As String, title As String, subtitle As String, imagePath As String, description As String)
            Me.UniqueId = uniqueId
            Me.Title = title
            Me.Subtitle = subtitle
            Me.Description = description
            Me.ImagePath = imagePath
            Me.Items = New ObservableCollection(Of QuizDataItem)()
        End Sub

        Private _uniqueId As String
        Public Property UniqueId As String
            Get
                Return _uniqueId
            End Get
            Private Set(value As String)
                _uniqueId = value
            End Set
        End Property

        Private _title As String
        Public Property Title As String
            Get
                Return _title
            End Get
            Private Set(value As String)
                _title = value
            End Set
        End Property

        Private _subtitle As String
        Public Property Subtitle As String
            Get
                Return _subtitle
            End Get
            Private Set(value As String)
                _subtitle = value
            End Set
        End Property

        Private _description As String
        Public Property Description As String
            Get
                Return _description
            End Get
            Private Set(value As String)
                _description = value
            End Set
        End Property

        Private _imagePath As String
        Public Property ImagePath As String
            Get
                Return _imagePath
            End Get
            Private Set(value As String)
                _imagePath = value
            End Set
        End Property

        Private _items As ObservableCollection(Of QuizDataItem)
        Public Property Items As ObservableCollection(Of QuizDataItem)
            Get
                Return _items
            End Get
            Private Set(value As ObservableCollection(Of QuizDataItem))
                _items = value
            End Set
        End Property


        Private _image As ImageSource = Nothing
        Public ReadOnly Property Image As ImageSource
            Get
                If Me._image Is Nothing AndAlso Me._imagePath IsNot Nothing Then
                    Me._image = New BitmapImage(New Uri(QuizDataGroup._baseUri, Me._imagePath))
                End If
                Return Me._image
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Me.Title
        End Function
    End Class

    ''' <summary>
    ''' Creates a collection of groups and items with content read from a static json file.
    ''' 
    ''' SampleDataSource initializes with data read from a static json file included in the 
    ''' project.  This provides sample data at both design-time and run-time.
    ''' </summary>
    Public NotInheritable Class QuizDataSource
        Private Shared _quizDataSource As New QuizDataSource()

        Private _groups As New ObservableCollection(Of QuizDataGroup)()
        Public ReadOnly Property Groups As ObservableCollection(Of QuizDataGroup)
            Get
                Return Me._groups
            End Get
        End Property

        Public Shared Async Function GetGroupsAsync() As Task(Of IEnumerable(Of QuizDataGroup))
            Await _quizDataSource.GetQuizDataAsync()
            Return _quizDataSource.Groups
        End Function

        Public Shared Async Function GetGroupAsync(uniqueId As String) As Task(Of QuizDataGroup)
            Await _quizDataSource.GetQuizDataAsync()
            ' Simple linear search is acceptable for small data sets
            Dim matches As IEnumerable(Of QuizDataGroup) = _quizDataSource.Groups.Where(Function(group) group.UniqueId.Equals(uniqueId))
            If matches.Count() = 1 Then Return matches.First()
            Return Nothing
        End Function

        Public Shared Async Function GetItemAsync(uniqueId As String) As Task(Of QuizDataItem)
            Await _quizDataSource.GetQuizDataAsync()
            ' Simple linear search is acceptable for small data sets
            Dim matches As IEnumerable(Of QuizDataItem) = _quizDataSource.Groups.SelectMany(Function(group) group.Items).Where(Function(item) item.UniqueId.Equals(uniqueId))
            If matches.Count() = 1 Then Return matches.First()
            Return Nothing
        End Function

        Private Async Function GetQuizDataAsync() As Task

            If Me._groups.Count <> 0 Then
                Return
            End If

            Dim dataUri As New Uri("ms-appx:///DataModel/QuizData.json")

            Dim file As StorageFile = Await StorageFile.GetFileFromApplicationUriAsync(dataUri)
            Dim jsonText As String = Await FileIO.ReadTextAsync(file)
            Dim jsonObject As JsonObject = jsonObject.Parse(jsonText)
            Dim jsonArray As JsonArray = jsonObject("Groups").GetArray()

            For Each groupValue As JsonValue In jsonArray
                Dim groupObject As JsonObject = groupValue.GetObject()
                Dim group As New QuizDataGroup(groupObject("UniqueId").GetString(), groupObject("Title").GetString(), groupObject("Subtitle").GetString(), groupObject("ImagePath").GetString(), groupObject("Description").GetString())

                For Each itemValue As JsonValue In groupObject("Items").GetArray()
                    Dim itemObject As JsonObject = itemValue.GetObject()
                    group.Items.Add(New QuizDataItem(itemObject("UniqueId").GetString(), itemObject("Question_Number").GetString(), itemObject("Question").GetString(), itemObject("Answer_1").GetString(), itemObject("Answer_2").GetString(),
                                                       itemObject("Answer_3").GetString(), itemObject("Correct").GetString()))
                Next

                Me.Groups.Add(group)
            Next
        End Function
    End Class
End Namespace