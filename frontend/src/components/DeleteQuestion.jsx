import { useState } from 'react';
import GetQuestionByIdForm from './GetQuestionByIdForm';
import QuestionForm from './QuestionForm';

const DeleteQuestion = ({ adjustGradient }) => {
    const [questionId, setQuestionId] = useState(0);
    const [getQuestionByIdButtonDisabled, setGetQuestionByIdButtonDisabled] =
        useState(true);
    const [deleteQuestionButtonDisabled, setDeleteQuestionButtonDisabled] =
        useState(true);
    const [formData, setFormData] = useState({
        questionType: '',
        question: '',
        option1: '',
        option2: '',
        option3: '',
        option4: '',
        correctOptionNumber: 1,
    });
    const [getQuestionMessageAndState, setGetQuestionMessageAndState] =
        useState({
            success: true,
            message: '',
        });
    const [deleteQuestionMessageAndState, setDeleteQuestionMessageAndState] =
        useState({
            success: true,
            message: '',
        });

    const handleGetQuestionFormChange = (e) => {
        const { value } = e.target;

        const intValue = Number(value);

        setQuestionId(intValue);
        setGetQuestionByIdButtonDisabled(intValue <= 0);
        setDeleteQuestionButtonDisabled(intValue <= 0);
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

                setFormData({
                    questionType: deserializedResponse.questionType,
                    question: deserializedResponse.question,
                    option1: deserializedResponse.option1,
                    option2: deserializedResponse.option2,
                    option3: deserializedResponse.option3,
                    option4: deserializedResponse.option4,
                    correctOptionNumber:
                        deserializedResponse.correctOptionNumber,
                });
            } else {
                const error = await response.text();
                setGetQuestionMessageAndState({
                    success: false,
                    message: error,
                });
                console.error('Failed to get question:', error);
            }
        } catch (exception) {
            setGetQuestionMessageAndState({
                success: false,
                message: exception.message,
            });
            console.error('An error occured:', exception.message);
        }
    };

    const handleDeleteQuestion = async (e) => {
        e.preventDefault();

        try {
            const response = await fetch(
                `https://localhost:7030/api/quiz/delete/${questionId}`,
                {
                    method: 'DELETE',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                }
            );

            if (response.ok) {
                adjustGradient();
                setDeleteQuestionMessageAndState({
                    success: true,
                    message: 'Sucesss!',
                });
            } else {
                const error = await response.text();
                setDeleteQuestionMessageAndState({
                    success: false,
                    message: error,
                });
                console.error('Failed to delete question:', error);
            }
        } catch (exception) {
            setDeleteQuestionMessageAndState({
                success: false,
                message: exception.message,
            });
            console.error('An error occured:', exception.message);
        }
    };

    return (
        <>
            <h1>Delete Question</h1>
            <h3>
                To delete a quesiton, please follow the instructions below...
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
                handleFormChange={null}
                submitFormButtonDisabled={deleteQuestionButtonDisabled}
                handleFormSubmit={handleDeleteQuestion}
                correctOptionNumber={formData.correctOptionNumber}
                isModifyingQuestion={false}
                disableAllInput={true}
            />

            <p
                className={
                    deleteQuestionMessageAndState.success
                        ? 'success-message'
                        : 'error-message'
                }>
                {deleteQuestionMessageAndState.message}
            </p>
        </>
    );
};

export default DeleteQuestion;
