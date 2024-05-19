import { useState } from 'react';
import GetQuestionByIdForm from './GetQuestionByIdForm';
import QuestionForm from './QuestionForm';
import quizService from '../services/quizService';

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
            const response = await quizService.getQuestionById(questionId);

            adjustGradient();
            setGetQuestionMessageAndState({
                success: true,
                message: 'Sucesss!',
            });

            const deserializedResponse = await response.data;

            setFormData({
                questionType: deserializedResponse.questionType,
                question: deserializedResponse.question,
                option1: deserializedResponse.option1,
                option2: deserializedResponse.option2,
                option3: deserializedResponse.option3,
                option4: deserializedResponse.option4,
                correctOptionNumber: deserializedResponse.correctOptionNumber,
            });
        } catch (exception) {
            const message =
                exception.response.data || exception.response.statusText;

            setGetQuestionMessageAndState({
                success: false,
                message: message,
            });
            console.error('An error occured:', message);
        }
    };

    const handleDeleteQuestion = async (e) => {
        e.preventDefault();

        try {
            await quizService.deleteQuestion(questionId);

            adjustGradient();
            setDeleteQuestionMessageAndState({
                success: true,
                message: 'Sucesss!',
            });
        } catch (exception) {
            const message =
                exception.response.data || exception.response.statusText;

            setDeleteQuestionMessageAndState({
                success: false,
                message: message,
            });
            console.error('An error occured:', message);
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
