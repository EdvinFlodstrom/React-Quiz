import { useState } from 'react';
import { formInputIsInvalid } from '../utils/createOrModifyQuestionUtils';
import GetQuestionByIdForm from './GetQuestionByIdForm';
import QuestionForm from './QuestionForm';
import quizService from '../services/quizService';

const ModifyQuestion = ({ adjustGradient }) => {
    const [getQuestionByIdButtonDisabled, setGetQuestionByIdButtonDisabled] =
        useState(true);
    const [questionId, setQuestionId] = useState(0);
    const [getQuestionMessageAndState, setGetQuestionMessageAndState] =
        useState({
            success: true,
            message: '',
        });
    const [modifyQuestionMessageAndState, setModifyQuestionMessageAndState] =
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
        setModifyQuestionButtonDisabled(intValue <= 0);
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
            setCorrectOptionNumber(deserializedResponse.correctOptionNumber);
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

    const handleFormChange = (e) => {
        const { name, value } = e.target;

        let newValue = value;
        if (name === 'correctOptionNumber') {
            newValue = Number(newValue);
            setCorrectOptionNumber(newValue);
        }
        const updatedFormData = { ...formData, [name]: newValue };

        setFormData(updatedFormData);
        setModifyQuestionButtonDisabled(
            formInputIsInvalid(updatedFormData) || questionId <= 0
        );
    };

    const handleFormSubmit = async (e) => {
        e.preventDefault();

        try {
            await quizService.modifyQuestion(questionId, formData);

            adjustGradient();
            setModifyQuestionMessageAndState({
                success: true,
                message: 'Success!',
            });
        } catch (exception) {
            const message =
                exception.response.data || exception.response.statusText;

            setModifyQuestionMessageAndState({
                success: false,
                message: message,
            });
            console.error('An error occured:', message);
        }
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
                submitFormButtonDisabled={modifyQuestionButtonDisabled}
                handleFormSubmit={handleFormSubmit}
                correctOptionNumber={correctOptionNumber}
                isModifyingQuestion={true}
                disableAllInput={false}
            />

            <p
                className={
                    modifyQuestionMessageAndState.success
                        ? 'success-message'
                        : 'error-message'
                }>
                {modifyQuestionMessageAndState.message}
            </p>
        </>
    );
};

export default ModifyQuestion;
