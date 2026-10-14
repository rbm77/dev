"use client";

import { useId } from "react";
import { useController, type Control, type FieldPath, type FieldValues } from "react-hook-form";
import { Field, FieldContent, FieldDescription, FieldError, FieldLabel } from "@/components/ui/field";
import { Input } from "@/components/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Switch } from "@/components/ui/switch";
import { Textarea } from "@/components/ui/textarea";
import { PasswordInput } from "@/components/shared/password-input";
import { cn } from "@/lib/utils";

// Form fields bound to react-hook-form. Values are kept as strings (inputs) and
// converted by the zod schema, which is shared with the Server Action.

export type Option = { value: string; label: string };

type BaseFieldProps<T extends FieldValues> = {
  // eslint-disable-next-line @typescript-eslint/no-explicit-any -- matches react-hook-form's own Control default
  control: Control<T, any, any>;
  name: FieldPath<T>;
  label: string;
  description?: string;
  required?: boolean;
  disabled?: boolean;
  className?: string;
};

function Label({ htmlFor, label, required }: { htmlFor: string; label: string; required?: boolean }) {
  return (
    <FieldLabel htmlFor={htmlFor}>
      {label}
      {required && (
        <span className="text-destructive" aria-hidden>
          *
        </span>
      )}
    </FieldLabel>
  );
}

function asText(value: unknown): string {
  return value === null || value === undefined ? "" : String(value);
}

type TextFieldProps<T extends FieldValues> = BaseFieldProps<T> & {
  maxLength?: number;
  type?: "text" | "email" | "tel" | "date" | "number";
  inputMode?: React.HTMLAttributes<HTMLInputElement>["inputMode"];
  autoComplete?: string;
  placeholder?: string;
  step?: string;
  min?: string;
  max?: string;
};

export function TextField<T extends FieldValues>({
  control,
  name,
  label,
  description,
  required,
  disabled,
  className,
  type = "text",
  ...inputProps
}: TextFieldProps<T>) {
  const id = useId();
  const {
    field: { ref, name: fieldName, value: fieldValue, onChange, onBlur },
    fieldState,
  } = useController({ control, name });

  return (
    <Field data-invalid={fieldState.invalid} className={className}>
      <Label htmlFor={id} label={label} required={required} />
      <Input
        id={id}
        type={type}
        disabled={disabled}
        aria-invalid={fieldState.invalid}
        aria-required={required}
        {...inputProps}
        name={fieldName}
        ref={ref}
        value={asText(fieldValue)}
        onChange={(event) => onChange(event.target.value)}
        onBlur={onBlur}
      />
      {description && <FieldDescription>{description}</FieldDescription>}
      <FieldError errors={[fieldState.error]} />
    </Field>
  );
}

/** Amounts in colones: numeric keyboard on phones, 2 decimals. */
export function MoneyField<T extends FieldValues>(props: BaseFieldProps<T>) {
  return <TextField {...props} type="number" inputMode="decimal" step="0.01" min="0" placeholder="0,00" />;
}

/** Discount percentage (0.01–100, 2 decimals). */
export function PercentageField<T extends FieldValues>(props: BaseFieldProps<T>) {
  return <TextField {...props} type="number" inputMode="decimal" step="0.01" min="0.01" max="100" />;
}

/** Whole numbers (years, liters, days...). */
export function IntegerField<T extends FieldValues>(props: BaseFieldProps<T> & { min?: string; max?: string }) {
  return <TextField {...props} type="number" inputMode="numeric" step="1" />;
}

/** Native date picker ("yyyy-MM-dd"), the best experience on phones. */
export function DateField<T extends FieldValues>(props: BaseFieldProps<T>) {
  return <TextField {...props} type="date" />;
}

type TextareaFieldProps<T extends FieldValues> = BaseFieldProps<T> & { maxLength: number; placeholder?: string };

export function TextareaField<T extends FieldValues>({
  control,
  name,
  label,
  description,
  required,
  disabled,
  className,
  maxLength,
  placeholder,
}: TextareaFieldProps<T>) {
  const id = useId();
  const {
    field: { ref, name: fieldName, value: fieldValue, onChange, onBlur },
    fieldState,
  } = useController({ control, name });
  const value = asText(fieldValue);

  return (
    <Field data-invalid={fieldState.invalid} className={className}>
      <Label htmlFor={id} label={label} required={required} />
      <Textarea
        id={id}
        rows={3}
        maxLength={maxLength}
        placeholder={placeholder}
        disabled={disabled}
        aria-invalid={fieldState.invalid}
        name={fieldName}
        ref={ref}
        value={value}
        onChange={(event) => onChange(event.target.value)}
        onBlur={onBlur}
      />
      <div className="flex justify-between gap-2">
        {description ? <FieldDescription>{description}</FieldDescription> : <span />}
        <span className="text-xs text-muted-foreground tabular-nums">
          {value.length}/{maxLength}
        </span>
      </div>
      <FieldError errors={[fieldState.error]} />
    </Field>
  );
}

type PasswordFieldProps<T extends FieldValues> = BaseFieldProps<T> & { maxLength: number; autoComplete?: string };

/** Secret input; never pre-filled by the app. */
export function PasswordField<T extends FieldValues>({
  control,
  name,
  label,
  description,
  required,
  disabled,
  className,
  maxLength,
  autoComplete = "new-password",
}: PasswordFieldProps<T>) {
  const id = useId();
  const {
    field: { ref, name: fieldName, value: fieldValue, onChange, onBlur },
    fieldState,
  } = useController({ control, name });

  return (
    <Field data-invalid={fieldState.invalid} className={className}>
      <Label htmlFor={id} label={label} required={required} />
      <PasswordInput
        id={id}
        maxLength={maxLength}
        autoComplete={autoComplete}
        disabled={disabled}
        aria-invalid={fieldState.invalid}
        name={fieldName}
        ref={ref}
        value={asText(fieldValue)}
        onChange={(event) => onChange(event.target.value)}
        onBlur={onBlur}
      />
      {description && <FieldDescription>{description}</FieldDescription>}
      <FieldError errors={[fieldState.error]} />
    </Field>
  );
}

type SelectFieldProps<T extends FieldValues> = BaseFieldProps<T> & {
  options: readonly Option[];
  placeholder?: string;
};

export function SelectField<T extends FieldValues>({
  control,
  name,
  label,
  description,
  required,
  disabled,
  className,
  options,
  placeholder = "Selecciona una opción",
}: SelectFieldProps<T>) {
  const id = useId();
  const {
    field: { ref, name: fieldName, value: fieldValue, onChange, onBlur },
    fieldState,
  } = useController({ control, name });
  const value = asText(fieldValue);

  return (
    <Field data-invalid={fieldState.invalid} className={className}>
      <Label htmlFor={id} label={label} required={required} />
      <Select
        value={value}
        onValueChange={(next) => onChange(next)}
        disabled={disabled || options.length === 0}
        name={fieldName}
      >
        <SelectTrigger id={id} ref={ref} onBlur={onBlur} aria-invalid={fieldState.invalid} className="w-full">
          <SelectValue placeholder={options.length === 0 ? "No hay opciones disponibles" : placeholder} />
        </SelectTrigger>
        <SelectContent>
          {options.map((option) => (
            <SelectItem key={option.value} value={option.value}>
              {option.label}
            </SelectItem>
          ))}
        </SelectContent>
      </Select>
      {description && <FieldDescription>{description}</FieldDescription>}
      <FieldError errors={[fieldState.error]} />
    </Field>
  );
}

/** On/off setting rendered as a labelled row (e.g. "Activo"). */
export function SwitchField<T extends FieldValues>({
  control,
  name,
  label,
  description,
  disabled,
  className,
}: Omit<BaseFieldProps<T>, "required">) {
  const id = useId();
  const {
    field: { ref, name: fieldName, value: fieldValue, onChange, onBlur },
  } = useController({ control, name });

  return (
    <Field orientation="horizontal" className={cn("rounded-lg border bg-card p-3", className)}>
      <FieldContent>
        <FieldLabel htmlFor={id}>{label}</FieldLabel>
        {description && <FieldDescription>{description}</FieldDescription>}
      </FieldContent>
      <Switch
        id={id}
        name={fieldName}
        ref={ref}
        checked={Boolean(fieldValue)}
        onCheckedChange={(checked) => onChange(checked)}
        onBlur={onBlur}
        disabled={disabled}
      />
    </Field>
  );
}
