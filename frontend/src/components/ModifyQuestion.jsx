import { useState } from 'react';
import GetQuestionByIdForm from './GetQuestionByIdForm';
import QuestionForm from './QuestionForm';

const ModifyQuestion = ({ adjustGradient }) => {
    const [getQuestionByIdButtonDisabled, setGetQuestionByIdButtonDisabled] =
        useState(true);
    const [questionId, setQuestionId] = useState(0);
    const [getQuestionMessageAndState, setGetQuestionMessageAndState] =
        useState({
            success: true,
            message: '',
        });
    const [formData, setFormData] = useState({
        questionType: '',
        question: '',
        option1: '',
        option2: '',
        option3: '',
        option4: '',
        correctOptionNumber: 1,
    });
    const [modifyQuestionButtonDisabled, setModifyQuestionButtonDisabled] =
        useState(true);
    const [correctOptionNumber, setCorrectOptionNumber] = useState(1);

    const handleGetQuestionFormChange = (e) => {
        const { value } = e.target;

        const intValue = Number(value);

        setQuestionId(intValue);
        setGetQuestionByIdButtonDisabled(intValue <= 0);
    };

    const handleGetQuestion = async (e) => {
        e.preventDefault();

        try {
            const response = await fetch(
                `https://localhost:7030/api/quiz/get/${questionId}`,
                {
                    method: 'GET',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                }
            );

            if (response.ok) {
                adjustGradient();
                setGetQuestionMessageAndState({
                    success: true,
                    message: 'Sucesss!',
                });
                const deserializedResponse = await response.json();
                console.log(deserializedResponse.question);
                console.log(deserializedResponse.option1);
                console.log(deserializedResponse.option2);
                console.log(deserializedResponse.option3);
                console.log(deserializedResponse.option4);
                console.log(deserializedResponse.correctOptionNumber);
            } else {
                const error = await response.text();
                setGetQuestionMessageAndState({
                    success: false,
                    message: error,
                });
                console.error('Failed to create question:', error);
            }
        } catch (exception) {
            setGetQuestionMessageAndState({
                success: false,
                message: exception.message,
            });
            console.error('An error occured:', exception.message);
        }
    };

    const handleFormChange = () => {};

    const handleFormSubmit = async (e) => {
        e.preventDefault();
    };

    return (
        <>
            <h1>Modify Question</h1>
            <h3>
                To modify a question, please follow the instructions below...
            </h3>

            <GetQuestionByIdForm
                handleGetQuestion={handleGetQuestion}
                questionId={questionId}
                handleGetQuestionFormChange={handleGetQuestionFormChange}
                getQuestionByIdButtonDisabled={getQuestionByIdButtonDisabled}
            />

            <p
                className={
                    getQuestionMessageAndState.success
                        ? 'success-message'
                        : 'error-message'
                }>
                {getQuestionMessageAndState.message}
            </p>

            <QuestionForm
                formData={formData}
                handleFormChange={handleFormChange}
                createOrModifyQuestionButtonDisabled={
                    modifyQuestionButtonDisabled
                }
                handleFormSubmit={handleFormSubmit}
                correctOptionNumber={correctOptionNumber}
                isModifyingQuestion={true}
            />
        </>
    );
};

export default ModifyQuestion;
