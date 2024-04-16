# LocalizationEditor

    [패치노트]
            v1.1.0  - 24.04.17
                    - Key type을 기존에 상수 문자열을 사용했지만, 이제 enum을 사용하여 키 관리를 하도록 구조 변경
                    - 관리 구조 변경에 의한 스크립트 코드 수정
                    - Localization Editor로 생성되는 스크립트 이름 변경
                        (LocalizationConstants -> LocalizationKeys)
                    
            v1.0.0  - 24.04.02
                    - Localization Editor 기능 구현

---------------------------------------------------------------------------------------------
![image](https://github.com/kastro723/LocalizationEditor/assets/55536937/30ea5a25-1d23-4458-bd5d-f3a1bdc7eb84)

![image](https://github.com/kastro723/LocalizationEditor/assets/55536937/d7f27a70-473d-4e75-9849-4d85b4386e71)

    [엑셀 데이터 형식 예시]
            1열 = id(key), 언어(kr, en, jp) - Key
            2열 = 데이터 타입(string)
            
            3열 ~ id(key) - 언어에 맞는 데이터 값(Value)
            


    [기능설명]
    
            Excel파일 (.xslx)의 첫 번째 열(id)을 대문자 스네이크 표기법 형식으로 상수로 선언하고, 열거형으로 저장되는 스크립트(Enum Script, LocalizationKeys.cs) 자동 생성

            LocalizationManger 싱글톤 클래스를 통해(instance) 해당 키(Key)를 받아 해당하는 문자열(Value) 반환
            
            LocalizationManager 클래스를 통한 언어(kr,en,jp) 변경 지원


    [사용방법]
    
            1. Drag and Drop을 통해 Excel(.xlsx)파일을 DragArea에 넣는다.

            2. 'Generate LocalizationKeys Class' 버튼을 클릭해 저장 폴더를 선택 및 스크립트 자동 생성

            3. LocalizationManager 클래스를 통해 언어 설정 및 키를 입력해 현재 설정된 언어와 상수 키에 맞는 매칭 값(문자열) 리턴


​            
