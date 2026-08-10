using System.Globalization;
using System.Windows.Data;

namespace System.Windows.Controls
{
	public class ValidationResult
	{
		private bool _isValid;

		private object _errorContent;

		private static readonly ValidationResult s_valid = new ValidationResult(isValid: true, null);

		//
		// Сводка:
		//     Возвращает значение, указывающее, является ли проверяться значение System.Windows.Controls.ValidationRule
		//     является допустимым.
		//
		// Возврат:
		//     true Если значение является допустимым; в противном случае — false. Значение
		//     по умолчанию — false.
		public bool IsValid => _isValid;

		//
		// Сводка:
		//     Возвращает объект, предоставляющий дополнительные сведения о недопустимости.
		//
		//
		// Возврат:
		//     Объект, предоставляющий дополнительные сведения о недопустимости.
		public object ErrorContent => _errorContent;

		//
		// Сводка:
		//     Возвращает допустимый экземпляр System.Windows.Controls.ValidationResult.
		//
		// Возврат:
		//     Допустимый экземпляр System.Windows.Controls.ValidationResult.
		public static ValidationResult ValidResult => s_valid;

		//
		// Сводка:
		//     Инициализирует новый экземпляр класса System.Windows.Controls.ValidationResult.
		//
		//
		// Параметры:
		//   isValid:
		//     Ли проверяться значение System.Windows.Controls.ValidationRule является допустимым.
		//
		//
		//   errorContent:
		//     Сведения о недопустимости.
		public ValidationResult(bool isValid, object errorContent)
		{
			_isValid = isValid;
			_errorContent = errorContent;
		}

		//
		// Сводка:
		//     Сравнивает два System.Windows.Controls.ValidationResult объектов на равенство
		//     значений.
		//
		// Параметры:
		//   left:
		//     Первый экземпляр для сравнения.
		//
		//   right:
		//     Второй экземпляр для сравнения.
		//
		// Возврат:
		//     Значение true, если эти два объекта равны; в противном случае — значение false.
		public static bool operator ==(ValidationResult left, ValidationResult right)
		{
			return object.Equals(left, right);
		}

		//
		// Сводка:
		//     Сравнивает два System.Windows.Controls.ValidationResult объектов на неравенство
		//     значений.
		//
		// Параметры:
		//   left:
		//     Первый экземпляр для сравнения.
		//
		//   right:
		//     Второй экземпляр для сравнения.
		//
		// Возврат:
		//     Значение true, если значения неравные; в противном случае — значение false.
		public static bool operator !=(ValidationResult left, ValidationResult right)
		{
			return !object.Equals(left, right);
		}

		//
		// Сводка:
		//     Проверяет указанный экземпляр и текущий экземпляр System.Windows.Controls.ValidationResult
		//     на равенство значений.
		//
		// Параметры:
		//   obj:
		//     Первый экземпляр System.Windows.Controls.ValidationResult для сравнения.
		//
		// Возврат:
		//     true Если obj и этот экземпляр System.Windows.Controls.ValidationResult.have
		//     те же значения.
		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}

			ValidationResult validationResult = obj as ValidationResult;
			if (validationResult != null)
			{
				if (IsValid == validationResult.IsValid)
				{
					return ErrorContent == validationResult.ErrorContent;
				}

				return false;
			}

			return false;
		}

		//
		// Сводка:
		//     Возвращает хэш-код для модуля чтения данных System.Windows.Controls.ValidationResult.
		//
		//
		// Возврат:
		//     Хэш-код для этого экземпляра System.Windows.Controls.ValidationResult.
		public override int GetHashCode()
		{
			return IsValid.GetHashCode() ^ ((ErrorContent == null) ? ((object)int.MinValue) : ErrorContent).GetHashCode();
		}
	}
	public enum ValidationStep
	{
		RawProposedValue
	}

	public abstract class ValidationRule
	{
		private ValidationStep _validationStep;

		private bool _validatesOnTargetUpdated;

		//
		// Сводка:
		//     Возвращает или задает, когда выполняется правило проверки.
		//
		// Возврат:
		//     Одно из значений перечисления. Значение по умолчанию — System.Windows.Controls.ValidationStep.RawProposedValue.
		public ValidationStep ValidationStep
		{
			get
			{
				return _validationStep;
			}
			set
			{
				_validationStep = value;
			}
		}

		//
		// Сводка:
		//     Возвращает или задает значение, указывающее, выполняется ли правило проверки
		//     при целевой System.Windows.Data.Binding обновляется.
		//
		// Возврат:
		//     true Если выполняется правило проверки, когда целевой System.Windows.Data.Binding
		//     обновленные; в противном случае — false.
		public bool ValidatesOnTargetUpdated
		{
			get
			{
				return _validatesOnTargetUpdated;
			}
			set
			{
				_validatesOnTargetUpdated = value;
			}
		}

		//
		// Сводка:
		//     Инициализирует новый экземпляр класса System.Windows.Controls.ValidationRule.
		protected ValidationRule()
			: this(ValidationStep.RawProposedValue, validatesOnTargetUpdated: false)
		{
		}

		//
		// Сводка:
		//     Инициализирует новый экземпляр System.Windows.Controls.ValidationRule с указанным
		//     шагом проверки и значение, указывающее, выполняется ли правило проверки, когда
		//     обновляется целевой объект.
		//
		// Параметры:
		//   validationStep:
		//     Одно из значений перечисления, указывающее, когда выполняется правило проверки.
		//
		//
		//   validatesOnTargetUpdated:
		//     true Чтобы выполнять, когда правило проверки целевой System.Windows.Data.Binding
		//     обновленные; в противном случае — false.
		protected ValidationRule(ValidationStep validationStep, bool validatesOnTargetUpdated)
		{
			_validationStep = validationStep;
			_validatesOnTargetUpdated = validatesOnTargetUpdated;
		}

		//
		// Сводка:
		//     При переопределении в производном классе выполняет проверку значения.
		//
		// Параметры:
		//   value:
		//     Проверяемое значение целевого объекта привязки.
		//
		//   cultureInfo:
		//     Язык и региональные параметры, используемые в правиле.
		//
		// Возврат:
		//     Объект System.Windows.Controls.ValidationResult.
		public abstract ValidationResult Validate(object value, CultureInfo cultureInfo);

		//
		// Сводка:
		//     Выполняет проверки по значению.
		//
		// Параметры:
		//   value:
		//     Проверяемое значение целевого объекта привязки.
		//
		//   cultureInfo:
		//     Язык и региональные параметры, используемые в правиле.
		//
		//   owner:
		//     Выражение привязки, который использует правила проверки.
		//
		// Возврат:
		//     Объект System.Windows.Controls.ValidationResult.
		public virtual ValidationResult Validate(object value, CultureInfo cultureInfo, BindingExpressionBase owner)
		{
			ValidationStep validationStep = _validationStep;
			if ((uint)(validationStep - 2) <= 1u)
			{
				value = owner;
			}

			return Validate(value, cultureInfo);
		}

		//
		// Сводка:
		//     Выполняет проверки по значению.
		//
		// Параметры:
		//   value:
		//     Проверяемое значение целевого объекта привязки.
		//
		//   cultureInfo:
		//     Язык и региональные параметры, используемые в правиле.
		//
		//   owner:
		//     Группа привязки, использует правила проверки.
		//
		// Возврат:
		//     Объект System.Windows.Controls.ValidationResult.
		public virtual ValidationResult Validate(object value, CultureInfo cultureInfo, BindingGroup owner)
		{
			return Validate(owner, cultureInfo);
		}
	}
}
